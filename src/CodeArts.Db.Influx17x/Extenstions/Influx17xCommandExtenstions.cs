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
            if (commandSql.Parameters is Dictionary<string, object> parameters)
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

                    sql = sql.Replace($"@{item.Key}@", parameterVal);
                }
            }

            return sql;
        }
    }
}
