namespace MyApp.ServiceModel.Common
{
    public class ColumnInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public ColumnDataType ColumnDataType { get; set; }

        /// <summary>
        /// 字段长度. 只在字符串或浮点类型时有效.  如果为0或null,表示 TEXT 类型.
        /// </summary>
        public int? FieldLength { get; set; }

        /// <summary>
        /// 精度,只对浮点型时有效，表示小数点位数.
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// 是否可以为null
        /// </summary>
        public bool CanNull { get; set; }

        /// <summary>
        /// 是否索引
        /// </summary>
        public bool IsIndex { get; set; }
    }
}