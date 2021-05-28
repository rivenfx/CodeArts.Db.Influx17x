using System.Reflection;

namespace CodeArts.Db
{
    /// <summary>
    /// influx1.7x列信息
    /// </summary>
    public class Influx17xColumnInfo
    {
        /// <summary>
        /// 列属性
        /// </summary>
        public virtual PropertyInfo Property { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        public virtual Influx17xColumnType ColumnType { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public virtual string ColumnName { get; set; }
    }
}
