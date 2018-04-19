using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MyApp.ServiceModel.Common;
using MyApp.ServiceModel.Models;
using ServiceStack.OrmLite;

namespace MyApp.Manage
{
    public class MysqlSchemaManage : ManageBase, ISchemaManage
    {
        // 新建业务表的基础结构
        private const string CreateTableTemplate = @"CREATE TABLE {0}(
            id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
            create_time DATETIME NOT NULL,
            create_user VARCHAR(15) NOT NULL,
            update_time DATETIME,
            update_user VARCHAR(15),
            record_state INT DEFAULT 0,
            row_hash VARCHAR(32)
        )";


        private const string RenameTableTemplate = @"RENAME TABLE {0} TO {1}";
        private const string DropTableTemplate = @"DROP TABLE IF EXISTS {0}";

        private const string TableExistsTemplte = @"SELECT COUNT(*)
            FROM information_schema.tables 
            WHERE table_schema = '{0}' 
            AND table_name = '{1}'";

        private const string TableColumnTemplte = @"SELECT *
            FROM information_schema.`COLUMNS` 
            WHERE table_schema = '{0}' 
            AND table_name = '{1}'";

        private const string ListTableTemplate = @"SELECT TABLE_NAME
           FROM information_schema.TABLES
           WHERE TABLE_SCHEMA = '{0}'";

        private const string AddColumnTemplte = @"ALTER TABLE {0} ADD COLUMN {1} {2}";
        private const string DeleteColumnTemplte = @"ALTER TABLE {0} DROP COLUMN {1}";
        private const string AddIndexTemplate = @"CREATE INDEX {0} ON {1}({2})";
        private const string DropIndexTemplate = @"DROP INDEX {0} ON {1}";
        private const string ChangeColumnTemplate = @"ALTER TABLE {0} CHANGE {1} {2} {3}";
        private const string RenameIndexTemplate = @"ALTER TABLE {0} RENAME INDEX {1} TO {2}";

        private async Task<IDbConnection> GetDbConnection(string dbName)
        {
            return await Task.FromResult(
                string.IsNullOrEmpty(dbName) || dbName == Constants.DefaultDatabaseName ||
                dbName == string.Format("{0}_test", Constants.DefaultDatabaseName)
                    ? DbFactory.Open()
                    : DbFactory.Open(dbName));
        }

        private async Task<string> GetDatabaseName(string dbName)
        {
            return await Task.FromResult(string.IsNullOrEmpty(dbName) || dbName == Constants.DefaultDatabaseName
                ? Constants.DefaultDatabaseName
                : dbName);
        }

        private async Task<string> GetIndexName(string tableName, string fieldName)
        {
            return await Task.FromResult(string.Format("idx_{0}_{1}", tableName, fieldName));
        }

        private async Task<string> GetColumnDefinition(ColumnInfo columnInfo)
        {
            string columnDef;
            switch (columnInfo.ColumnDataType)
            {
                case ColumnDataType.Date:
                    columnDef = "datetime";
                    break;
                case ColumnDataType.Integer:
                    columnDef = "bigint";
                    break;
                case ColumnDataType.Float:
                    columnDef = string.Format("double({0}, {1})", columnInfo.FieldLength, columnInfo.Precision);
                    break;
                case ColumnDataType.String:
                    columnDef = columnInfo.FieldLength.HasValue
                        ? string.Format("varchar({0})", columnInfo.FieldLength)
                        : "text";
                    break;
                default:
                    throw new UserFriendlyException("未定义的数据类型映射.");
            }

            return await Task.FromResult(columnDef);
        }

        public async Task CreateNormalTable(string tableName, string dbName = "")
        {
            var db = await GetDbConnection(dbName);
            var sql = string.Format(CreateTableTemplate, tableName);
            using (db)
            {
                using (var trans = db.OpenTransaction())
                {
                    await db.ExecuteNonQueryAsync(sql);

                    trans.Commit();
                }
            }
        }

        public async Task RenameIndex(string tableName, string oldIndexName, string newIndexName, string dbName = "")
        {
            var db = await GetDbConnection(dbName);
            var sql = string.Format(RenameIndexTemplate, tableName, oldIndexName, newIndexName);
            using (db)
            {
                await db.ExecuteNonQueryAsync(sql);
            }
        }


        public async Task ChangeTableName(string oldTableName, string newTablename, string dbName = "")
        {
            var db = await GetDbConnection(dbName);
            var sql = string.Format(RenameTableTemplate, oldTableName, newTablename);
            using (db)
            {
                await db.ExecuteNonQueryAsync(sql);
            }
        }

        public async Task<bool> TableExists(string tableName, string dbName = "")
        {
            var db = await GetDbConnection(dbName);
            var schema = await GetDatabaseName(dbName);
            var sql = string.Format(TableExistsTemplte, schema, tableName);
            using (db)
            {
                var count = await db.ScalarAsync<int>(sql);
                return count > 0;
            }
        }

        public async Task DropTable(string tableName, string dbName = "")
        {
            var db = await GetDbConnection(dbName);
            using (db)
            {
                var sql = string.Format(DropTableTemplate, tableName);
                await db.ExecuteNonQueryAsync(sql);
            }
        }

        public async Task<List<string>> GetTables(string dbName = "")
        {
            var schema = await GetDatabaseName(dbName);
            var db = await GetDbConnection(dbName);
            var sql = string.Format(ListTableTemplate, schema);
            using (db)
            {
                return await db.ColumnAsync<string>(sql);
            }
        }

        public async Task<List<TableField>> GetTableColumns(string tableName, string dbName = "")
        {
            var db = await GetDbConnection(dbName);
            var database = await GetDatabaseName(dbName);
            var sql = string.Format(TableColumnTemplte, database, tableName);
            using (db)
            {
                return await db.SelectAsync<TableField>(sql);
            }
        }

        public async Task<string> GetLogTableName(string tableName)
        {
            return await Task.FromResult($"{tableName}_log");
        }

        public async Task AddColumn(TableInfo tableInfo, ColumnInfo columnInfo)
        {
            var db = await GetDbConnection(tableInfo.DatabaseName);

            var colDef = await GetColumnDefinition(columnInfo);
            using (db)
            {
                using (var trans = db.OpenTransaction())
                {
                    var sql = string.Format(AddColumnTemplte, tableInfo.TableName, columnInfo.FieldName,
                        colDef);
                    var logTableName = await GetLogTableName(tableInfo.TableName);
                    var sqlLog = string.Format(AddColumnTemplte, logTableName, columnInfo.FieldName, colDef);

                    await db.ExecuteNonQueryAsync(sql);
                    await db.ExecuteNonQueryAsync(sqlLog);

                    if (columnInfo.IsIndex)
                    {
                        var indexName = await GetIndexName(tableInfo.TableName, columnInfo.FieldName);
                        var indexSql = string.Format(AddIndexTemplate, indexName, tableInfo.TableName,
                            columnInfo.FieldName);

                        await db.ExecuteNonQueryAsync(indexSql);
                    }

                    trans.Commit();
                }
            }
        }

        public async Task AlterColumn(TableInfo tableInfo, ColumnInfo oldColumnInfo, ColumnInfo newColumnInfo)
        {
            // 只有索引发生变更, 字段名发生变更, 数据类型或长度,精度发生变更时才进行操作. 否则没有变化不需要操作.
            if (oldColumnInfo.FieldName == newColumnInfo.FieldName &&
                oldColumnInfo.IsIndex == newColumnInfo.IsIndex &&
                oldColumnInfo.ColumnDataType == newColumnInfo.ColumnDataType &&
                oldColumnInfo.FieldLength == newColumnInfo.FieldLength &&
                oldColumnInfo.Precision == newColumnInfo.Precision)
            {
                return;
            }

            var db = await GetDbConnection(tableInfo.DatabaseName);

            var colDef = await GetColumnDefinition(newColumnInfo);

            using (db)
            {
                using (var trans = db.OpenTransaction())
                {
                    var sql = string.Format(ChangeColumnTemplate, tableInfo.TableName,
                        oldColumnInfo.FieldName, newColumnInfo.FieldName, colDef);

                    await db.ExecuteNonQueryAsync(sql);

                    // 如果字段名发生变更或表名发生变更,并且该字段是索引字段时,调整索引
                    if (oldColumnInfo.IsIndex && !newColumnInfo.IsIndex)
                    {
                        var indexName = await GetIndexName(tableInfo.TableName, oldColumnInfo.FieldName);
                        var dropIndexSql = string.Format(DropIndexTemplate, indexName, tableInfo.TableName);

                        await db.ExecuteNonQueryAsync(dropIndexSql);
                    }

                    if (!oldColumnInfo.IsIndex && newColumnInfo.IsIndex)
                    {
                        var indexName = await GetIndexName(tableInfo.TableName, newColumnInfo.FieldName);
                        var createIndexSql = string.Format(AddIndexTemplate, indexName, tableInfo.TableName,
                            newColumnInfo.FieldName);

                        await db.ExecuteNonQueryAsync(createIndexSql);
                    }
                    else if (newColumnInfo.IsIndex && oldColumnInfo.FieldName != newColumnInfo.FieldName)
                    {
                        var oldIndexName = await GetIndexName(tableInfo.TableName, oldColumnInfo.FieldName);
                        var newIndexName = await GetIndexName(tableInfo.TableName, newColumnInfo.FieldName);
                        var changeIndexSql = string.Format(RenameIndexTemplate, tableInfo.TableName,
                            oldIndexName, newIndexName);

                        await db.ExecuteNonQueryAsync(changeIndexSql);
                    }

                    trans.Commit();
                }
            }
        }

        public async Task DeleteColumn(TableInfo tableInfo, ColumnInfo columnInfo)
        {
            var db = await GetDbConnection(tableInfo.DatabaseName);

            using (db)
            {
                using (var trans = db.OpenTransaction())
                {
                    var sql = string.Format(DeleteColumnTemplte, tableInfo.TableName, columnInfo.FieldName);

                    if (columnInfo.IsIndex)
                    {
                        var indexName = await GetIndexName(tableInfo.TableName, columnInfo.FieldName);
                        var dropIndexSql = string.Format(DropIndexTemplate, indexName, tableInfo.TableName);

                        await db.ExecuteNonQueryAsync(dropIndexSql);
                    }

                    await db.ExecuteNonQueryAsync(sql);

                    trans.Commit();
                }
            }
        }

        private static Type GetColumnDataType(TableField field)
        {
            var dataType = field.DataType.ToLowerInvariant();
            var nullable = field.IsNullable.ToLowerInvariant() == "yes";

            switch (dataType)
            {
                case "int":
                case "bigint":
                    return nullable ? typeof(long?) : typeof(long);
                case "varchar":
                case "text":
                    return typeof(string);
                case "float":
                case "double":
                    return nullable ? typeof(double?) : typeof(double);
                case "datetime":
                    return nullable ? typeof(DateTime?) : typeof(DateTime);
                default:
                    return typeof(string);
            }
        }

        public async Task<EntityDef> ConvertToEntityDef(List<TableField> tableFields)
        {
            if (tableFields == null || tableFields.Count == 0)
            {
                return null;
            }

            var tableName = tableFields[0].TableName;
            return await Task.FromResult(new EntityDef
            {
                Name = tableName,
                Properties = tableFields.Select(field => new EntityProperty
                {
                    Name = field.FieldName,
                    Type = GetColumnDataType(field)
                }).ToList()
            });
        }

        public async Task<object> SingleById(Type entityType, object id)
        {
            using (var db = DbFactory.Open())
            {
                var method = typeof(OrmLiteReadApi).GetMethod("SingleById").MakeGenericMethod(entityType);

                var obj = method.Invoke(null, new[] {db, id});

                return await Task.FromResult(obj);
            }
        }

        public void SaveEntity(Type entityType, object obj)
        {
            using (var db = DbFactory.Open())
            {
                var method = typeof(OrmLiteWriteApi).GetMethods()
                    .First(x => x.Name == "Save" && x.GetParameters().Length == 3);

                var genMethod = method.MakeGenericMethod(entityType);

                genMethod.Invoke(null, new[] {db, obj, false});
            }
        }
    }
}