using CodeArts.Db.Lts;

using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArts.Db.Extenstions
{
    public static class Influx17xCommandSqlExtenstions
    {
        public static string ToInflux17xSql(this CommandSql commandSql)
        {
            var sql = commandSql.Sql;
            var parameters = commandSql.Parameters as Dictionary<string, object>;

            return ToInflux17xSql(sql, parameters);
        }


        public static string ToInflux17xSql(this IQueryVisitor queryVisitor)
        {
            return ToInflux17xSql(queryVisitor.ToSQL(), queryVisitor.Parameters);
        }

        public static string ToInflux17xSql(string sql, Dictionary<string, object> parameters)
        {
            var replaceM = false;
            if (parameters != null)
            {
                // 替换参数
                var parameterVal = string.Empty;
                foreach (var item in parameters)
                {
                    if (item.Value is DateTime dt)
                    {
                        parameterVal = dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                    }
                    else
                    {
                        parameterVal = item.Value.ToString();
                    }

                    replaceM = false;
                    if (item.Value is byte
                        || item.Value is float
                        || item.Value is double
                        || item.Value is decimal
                        || item.Value is long
                        || item.Value is ulong
                        || item.Value is uint
                        || item.Value is int
                        )
                    {
                        replaceM = true;
                    }


                    if (replaceM)
                    {
                        sql = sql.Replace($"'@{item.Key}@'", parameterVal);
                    }
                    else
                    {
                        sql = sql.Replace($"@{item.Key}@", parameterVal);
                    }
                }
            }

            return sql;
        }
    }
}
