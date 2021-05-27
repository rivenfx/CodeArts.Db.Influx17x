﻿using System.Collections.Generic;
using System.Text;

namespace CodeArts.Db
{
    public class Influx17xCorrectSettings : ISQLCorrectSettings
    {
        /// <summary>
        /// 字符串截取。 SUBSTRING。
        /// </summary>
        public string Substring => "SUBSTRING";

        /// <summary>
        /// 字符串索引。 LOCATE。
        /// </summary>
        public string IndexOf => "LOCATE";

        /// <summary>
        /// 字符串长度。 LENGTH。
        /// </summary>
        public string Length => "LENGTH";

        /// <summary>
        /// 索引项是否对调。 true。
        /// </summary>
        public bool IndexOfSwapPlaces => true;

        /// <summary>
        /// MySQL。
        /// </summary>
        public DatabaseEngine Engine => DatabaseEngine.Normal;

        private ICollection<IFormatter> formatters;
        /// <summary>
        /// 格式化集合。
        /// </summary>
        public ICollection<IFormatter> Formatters => formatters ?? (formatters = new List<IFormatter>());

        /// <summary>
        /// 字段名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns></returns>
        public string Name(string name) => string.Concat("\"", name.ToLower(), "\"");

        /// <summary>
        /// 参数名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns></returns>
        public string ParamterName(string name) => string.Concat("'@", name, "@'");

        /// <summary>
        /// SQL。
        /// </summary>
        /// <param name="sql">SQL。</param>
        /// <param name="take">获取“<paramref name="take"/>”条数据。</param>
        /// <param name="skip">跳过“<paramref name="skip"/>”条数据。</param>
        /// <param name="orderBy">排序。</param>
        /// <returns></returns>
        public virtual string ToSQL(string sql, int take, int skip, string orderBy)
        {
            var sb = new StringBuilder();

            sb.Append(sql)
                .Append(orderBy)
                .Append(" limit ")
                .Append(take)
                ;

            if (skip > 0)
            {
                sb.Append(" offset ")
                    .Append(skip);
            }

            return sb.ToString();
        }
    }
}
