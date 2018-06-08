using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DayuCloud.Common;
using DayuCloud.Manage;
using DayuCloud.Models;
using MyApp.ServiceModel;
using MyApp.ServiceModel.User;
using ServiceStack;
using ServiceStack.OrmLite;

namespace MyApp.ServiceInterface
{
    public class BizService : Service
    {
        public IAutoQueryDb AutoQuery { get; set; }

        public IReflectionManage ReflectionManage { get; set; }

        public ISchemaManage SchemaManage { get; set; }

        private async Task<TypeInfo> CreateBizType(string tableName)
        {
            var fields = await SchemaManage.GetTableColumns(tableName);
            var entityDef = await SchemaManage.ConvertToEntityDef(fields);
            if (entityDef == null)
            {
                throw new UserFriendlyException("表信息不存在。");
            }

            return await ReflectionManage.CreateEntityType(entityDef);
        }

        /// <summary>
        /// 基于 AutoQuery 实现统一的业务表动态查询。
        /// <see cref="http://docs.servicestack.net/autoquery-rdbms"/>
        /// </summary>
        /// <param name="query">包含显示表名及隐式的其它查询条件</param>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<IQueryResponse> Get(GetBizs query)
        {
            if (string.IsNullOrEmpty(query.TableName))
            {
                throw new UserFriendlyException("表参数不能为空。");
            }

            if (!query.TableName.StartsWith("biz_"))
            {
                throw new UserFriendlyException("只能查询业务表。");
            }

            // 根据表名得到表结构，并且根据表结构动态生成实体
            var entityType = await CreateBizType(query.TableName);

            // 动态生成用于 AutoQuery 的请求对象, 必须继承自 QueryDb<>
            // TODO: 通过设置 NamedConnection 可以自动实现分库的查询
            var routeBaseType = typeof(QueryDb<>).MakeGenericType(entityType);
            var routeType = await ReflectionManage.CreateEntityType(new EntityDef
            {
                Name = $"{query.TableName}_route",
                BaseType = routeBaseType,
                Properties = Enumerable.Empty<EntityProperty>()
            });

            var queryObj = Activator.CreateInstance(routeType);

            // 设置分页参数
            if (query.Skip.HasValue && query.Take.HasValue)
            {
                ((QueryBase) queryObj).Skip = query.Skip;
                ((QueryBase) queryObj).Take = query.Take;
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                ((QueryBase) queryObj).OrderBy = query.OrderBy;
            }

            if (!string.IsNullOrEmpty(query.OrderByDesc))
            {
                ((QueryBase) queryObj).OrderByDesc = query.OrderByDesc;
            }

            // 调用 AutoQuery 实现动态表查询
            var q = AutoQuery.CreateQuery((IQueryDb) queryObj, Request.GetRequestParams(), Request);
            var result = AutoQuery.Execute((IQueryDb) queryObj, q);

            return await Task.FromResult(result);
        }

        /// <summary>
        /// 新增或编辑一条业务记录
        /// </summary>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<long> Post(SaveBiz request)
        {
            if (string.IsNullOrEmpty(request.TableName) || !request.TableName.StartsWith("biz_"))
            {
                throw new UserFriendlyException("参数错误");
            }

            if (!IsAuthenticated)
            {
                throw new UserFriendlyException("未授权");
            }

            var userId = long.Parse(GetSession().UserAuthId);
            var user = await Db.SingleByIdAsync<UserEntity>(userId);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在。");
            }

            var entityType = await CreateBizType(request.TableName);
            object dbObj = null;
            long id = 0;

            var reqParam = Request.GetRequestParams();
            var isNew = true;
            if (reqParam.ContainsKey("id") && !string.IsNullOrEmpty(reqParam["id"]) && reqParam["id"] != "0")
            {
                id = long.Parse(reqParam["id"]);
                if (id > 0)
                {
                    isNew = false;
                    // 说明是更新操作， 先保存原数据到log表
                    var logEntityType = await CreateBizType(await SchemaManage.GetLogTableName(request.TableName));
                    dbObj = await SchemaManage.SingleById(entityType, id);

                    var logEntityObj = Activator.CreateInstance(logEntityType);
                    logEntityObj.PopulateWith(dbObj);

                    // 设置备份表的主表记录id
                    logEntityType.GetProperty("p_id").SetValue(logEntityObj, id);

                    SchemaManage.SaveEntity(logEntityType, logEntityObj);
                }
            }

            var entityObj = Activator.CreateInstance(entityType);
            if (dbObj != null)
            {
                // 如果是更新操作，先复制原始数据到新对象
                entityObj.PopulateInstance(dbObj);
            }

            foreach (var propInfo in entityType.GetProperties())
            {
                if (!propInfo.CanWrite)
                {
                    continue;
                }

                #region 处理预设字段

                if (!isNew)
                {
                    // 编辑时不保存 创建时间和创建人
                    if (propInfo.Name == "create_time" || propInfo.Name == "create_user")
                    {
                        continue;
                    }

                    // 记录状态，0为新增，1为编辑，2为删除
                    if (propInfo.Name == "record_state")
                    {
                        propInfo.SetValue(entityObj, 1L);
                        continue;
                    }

                    if (propInfo.Name == "update_time")
                    {
                        propInfo.SetValue(entityObj, DateTime.UtcNow);
                        continue;
                    }

                    if (propInfo.Name == "update_user")
                    {
                        propInfo.SetValue(entityObj, user.DisplayName);
                        continue;
                    }
                }
                else
                {
                    // 新增时操作， 修改时间和个修改人
                    if (propInfo.Name == "update_time" || propInfo.Name == "update_user")
                    {
                        continue;
                    }

                    if (propInfo.Name == "record_state")
                    {
                        propInfo.SetValue(entityObj, 0L);
                        continue;
                    }

                    if (propInfo.Name == "create_time")
                    {
                        propInfo.SetValue(entityObj, DateTime.UtcNow);
                        continue;
                    }

                    if (propInfo.Name == "create_user")
                    {
                        propInfo.SetValue(entityObj, user.DisplayName);
                        continue;
                    }
                }

                #endregion

                if (reqParam.ContainsKey(propInfo.Name))
                {
                    object propValue = reqParam[propInfo.Name];

                    // TODO: 转换
                    propInfo.SetValue(entityObj, propValue);
                }
            }

            SchemaManage.SaveEntity(entityType, entityObj);

            return id;
        }

        /// <summary>
        /// 删除一条业务记录
        /// </summary>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<long> Delete(DeleteBiz request)
        {
            if (string.IsNullOrEmpty(request.TableName))
            {
                throw new UserFriendlyException("参数错误");
            }

            if (!IsAuthenticated)
            {
                throw new UserFriendlyException("未授权");
            }

            var userId = long.Parse(GetSession().UserAuthId);
            var user = await Db.SingleByIdAsync<UserEntity>(userId);

            if (user == null)
            {
                throw new UserFriendlyException("用户不存在。");
            }

            var entityType = await CreateBizType(request.TableName);
            var entityObj = await SchemaManage.SingleById(entityType, request.Id);
            if (entityObj == null)
            {
                throw new UserFriendlyException("记录不存在。");
            }

            // 删除记录只打个标记, 值为2, 并更新修改字段
            entityType.GetProperty("record_state").SetValue(entityObj, 2);
            entityType.GetProperty("update_time").SetValue(entityObj, DateTime.UtcNow);
            entityType.GetProperty("update_user").SetValue(entityObj, user.DisplayName);

            SchemaManage.SaveEntity(entityType, entityObj);

            return await Task.FromResult(request.Id);
        }
    }
}