using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DayuCloud.Common;
using DayuCloud.Manage;
using MyApp.ServiceModel.Org;
using ServiceStack.OrmLite;

namespace MyApp.Manage
{
    public class OrgManage : ManageBase, IOrgManage
    {
        public async Task<List<OrganizationEntity>> GetRootOrganizations()
        {
            using (var db = DbFactory.Open())
            {
                return await db.SelectAsync<OrganizationEntity>(x => x.ParentId == 0);
            }
        }

        private static async Task<List<OrganizationEntity>> SetOrganizationInfo(IDbConnection db,
            List<OrganizationEntity> orgs)
        {
            if (orgs == null || orgs.Count == 0)
            {
                return null;
            }

            var parentIds = orgs.Select(x => x.ParentId).ToList();
            if (parentIds.Count == 0)
            {
                return null;
            }

            var parentOrgs = await db.SelectAsync<OrganizationEntity>(x => Sql.In(x.Id, parentIds));
            orgs.ForEach(org =>
            {
                var parent = parentOrgs.FirstOrDefault(x => x.Id == org.ParentId);
                if (parent != null)
                {
                    org.ParentName = parent.Name;
                }
            });
            return orgs;
        }

        public async Task<OrganizationEntity> GetOrganizationById(long id)
        {
            using (var db = DbFactory.Open())
            {
                var org = await db.SingleByIdAsync<OrganizationEntity>(id);
                if (org == null)
                {
                    return null;
                }

                var orgs = await SetOrganizationInfo(db, new List<OrganizationEntity> {org});

                return orgs?[0];
            }
        }

        public async Task<PagedResult<OrganizationEntity>> GetOrganizations(OrgQuery query)
        {
            using (var db = DbFactory.Open())
            {
                var builder = db.From<OrganizationEntity>();
                if (query.ParentId.HasValue && query.ParentId >= 0)
                {
                    builder.Where(x => x.ParentId == query.ParentId);
                }

                if (!string.IsNullOrEmpty(query.SearchName))
                {
                    builder.Where(x => x.Name.Contains(query.SearchName));
                }

                builder.OrderBy(x => x.ParentId).ThenBy(x => x.Id);

                var total = await db.CountAsync(builder);
                builder.Limit(query.Skip, query.PageSize);
                var results = await db.SelectAsync(builder);
                var orgs = await SetOrganizationInfo(db, results);

                return new PagedResult<OrganizationEntity> {Total = total, Results = orgs};
            }
        }

        public async Task<long> SaveOrganization(OrganizationEntity organizationEntity)
        {
            using (var db = DbFactory.Open())
            {
                // 同一级下的组织名称不能重复
                if (organizationEntity.Id > 0)
                {
                }

                var invalid = organizationEntity.Id > 0
                    ? await db.ExistsAsync<OrganizationEntity>(x => x.ParentId == organizationEntity.ParentId &&
                                                                    x.Name == organizationEntity.Name &&
                                                                    x.Id != organizationEntity.Id)
                    : await db.ExistsAsync<OrganizationEntity>(x => x.ParentId == organizationEntity.ParentId &&
                                                                    x.Name == organizationEntity.Name);
                if (invalid)
                {
                    throw new UserFriendlyException("同一级组织下名称不能重复。");
                }

                if (organizationEntity.Id > 0)
                {
                    // 只能更新父组织id和名称
                    await db.UpdateOnlyAsync(organizationEntity, onlyFields: x => new {x.ParentId, x.Name},
                        where: x => x.Id == organizationEntity.Id);
                }
                else
                {
                    organizationEntity.Id = await db.InsertAsync(organizationEntity, true);
                }

                return organizationEntity.Id;
            }
        }

        public async Task DeleteOrganization(long id)
        {
            using (var db = DbFactory.Open())
            {
                var hasChilds = await db.ExistsAsync<OrganizationEntity>(x => x.ParentId == id);
                if (hasChilds)
                {
                    throw new UserFriendlyException("该组织还有子组织，不能删除。必须先删除子组织。");
                }

                var c = await db.DeleteByIdAsync<OrganizationEntity>(id);
                if (c != 1)
                {
                    throw new UserFriendlyException("删除失败.");
                }
            }
        }
    }
}