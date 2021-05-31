
using System;
using System.Text;

namespace CodeArts.Db
{
    /// <summary>
    /// 列类型标记
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class Influx17xColumnAttribute : Attribute
    {
        /// <summary>
        /// 列类型
        /// </summary>
        public Influx17xColumnType Type { get; set; }

        public Influx17xColumnAttribute(Influx17xColumnType type)
        {
            Type = type;
        }
    }

    /// <summary>
    /// 列类型
    /// </summary>
    public enum Influx17xColumnType
    {
        /// <summary>
        /// 字段
        /// </summary>
        Field = 0,
        /// <summary>
        /// tag
        /// </summary>
        Tag = 1,
        /// <summary>
        /// 时间
        /// </summary>
        Timestamp = 2,
        /// <summary>
        /// 忽略
        /// </summary>
        Ignore = 3
    }
}
