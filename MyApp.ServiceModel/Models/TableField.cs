using ServiceStack.DataAnnotations;

namespace MyApp.ServiceModel.Models
{
    /// <summary>
    /// 代表数据库中表字段的实体
    /// </summary>
    public class TableField
    {
        /// <summary>
        /// 库名
        /// </summary>
        [Alias("TABLE_SCHEMA")]
        public string SchemaName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Alias("TABLE_NAME")]
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [Alias("COLUMN_NAME")]
        public string FieldName { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        [Alias("IS_NULLABLE")]
        public string IsNullable { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [Alias("DATA_TYPE")]
        public string DataType { get; set; }

        /// <summary>
        /// 字符串最大长度
        /// </summary>
        [Alias("CHARACTER_MAXIMUM_LENGTH")]
        public int? MaxCharLength { get; set; }

        /// <summary>
        /// 浮点类型的最大位数
        /// </summary>
        [Alias("NUMERIC_PERCISION")]
        public int? Percision { get; set; }

        /// <summary>
        /// 浮点类型的精度
        /// </summary>
        [Alias("NUMERIC_SCALE")]
        public int? Scale { get; set; }

        /// <summary>
        /// 完整的字段类型
        /// </summary>
        [Alias("COLUMN_TYPE")]
        public string FullColumnType { get; set; }
    }
}