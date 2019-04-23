using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyApp.ServiceModel.Common;

namespace MyApp.Manage
{
    /// <summary>
    /// 表结构服务
    /// </summary>
    public interface ISchemaManage
    {
        /// <summary>
        /// 创建包含自增id主键的表结构.
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbName">注册的数据库名. 为空表示默认库.</param>
        Task CreateNormalTable(string tableName, string dbName = "");

        /// <summary>
        /// 修改表名
        /// </summary>
        /// <param name="oldTableName">旧表名</param>
        /// <param name="newTablename">新表名</param>
        /// <param name="dbName">可空的注册数据库名.</param>
        Task ChangeTableName(string oldTableName, string newTablename, string dbName = "");

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbName">可能的库名</param>
        /// <returns>是否存在</returns>
        Task<bool> TableExists(string tableName, string dbName = "");

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbName">可能的库名</param>
        Task DropTable(string tableName, string dbName = "");

        /// <summary>
        /// 根据库名返回所有的表名
        /// </summary>
        /// <param name="dbName">可选的数据库名.</param>
        /// <returns>该库下的表名列表</returns>
        Task<List<string>> GetTables(string dbName = "");

        /// <summary>
        /// 得到业务表的备份表名
        /// </summary>
        /// <param name="tableName">业务表名</param>
        /// <returns>备份的业务表名</returns>
        Task<string> GetLogTableName(string tableName);

        /// <summary>
        /// 根据表名和库名得到该表的结构信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbName">可选的库名</param>
        /// <returns>表字段列表</returns>
        Task<List<TableField>> GetTableColumns(string tableName, string dbName = "");

        /// <summary>
        /// 根据数据项信息添加字段
        /// </summary>
        /// <param name="tableInfo">表信息</param>
        /// <param name="columnInfo">字段信息</param>
        Task AddColumn(TableInfo tableInfo, ColumnInfo columnInfo);

        /// <summary>
        /// 修改字段
        /// </summary>
        /// <param name="tableInfo">表信息</param>
        /// <param name="oldColumnInfo">旧字段信息</param>
        /// <param name="newColumnInfo">新字段信息</param>
        /// <remarks>修改可能会失败,如果数据类型发生变更时.</remarks>
        Task AlterColumn(TableInfo tableInfo, ColumnInfo oldColumnInfo, ColumnInfo newColumnInfo);

        /// <summary>
        /// 删除字段.
        /// </summary>
        /// <param name="tableInfo">表信息</param>
        /// <param name="columnInfo">字段信息</param>
        /// <returns></returns>
        Task DeleteColumn(TableInfo tableInfo, ColumnInfo columnInfo);

        /// <summary>
        /// 重命名索引
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="oldIndexName">旧索引名称</param>
        /// <param name="newIndexName">新索引名称</param>
        /// <param name="dbName">可选的数据库名</param>
        Task RenameIndex(string tableName, string oldIndexName, string newIndexName, string dbName = "");

        /// <summary>
        /// 根据表结构信息转换成实体模型属性信息
        /// </summary>
        /// <param name="tableFields">表字段列表</param>
        /// <returns>模型属性信息</returns>
        Task<EntityDef> ConvertToEntityDef(List<TableField> tableFields);

        /// <summary>
        /// 根据指定类型和id得到单条记录
        /// </summary>
        /// <param name="entiType">实体的类型</param>
        /// <param name="id">id值</param>
        /// <returns>实体的对象</returns>
        Task<object> SingleById(Type entiType, object id);

        /// <summary>
        /// 保存一个实体数据
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="obj">实体对象</param>
        /// <returns>保存后的id</returns>
        void SaveEntity(Type entityType, object obj);
    }
}