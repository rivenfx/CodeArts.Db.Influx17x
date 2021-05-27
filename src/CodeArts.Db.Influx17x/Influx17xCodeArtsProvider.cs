using CodeArts.Db.Ado;
using CodeArts.Db.Extenstions;
using CodeArts.Db.Lts;

using InfluxData.Net.InfluxDb.Models.Responses;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArts.Db
{
    public class Influx17xCodeArtsProvider : CodeArtsProvider
    {
        readonly ISQLCorrectSettings _settings;
        readonly ICustomVisitorList _visitors;
        public Influx17xCodeArtsProvider(ISQLCorrectSettings settings, ICustomVisitorList visitors) : base(settings, visitors)
        {
            _settings = settings;
            _visitors = visitors;
        }

        public override IQueryVisitor Create()
        {
            return new Influx17xQueryVisitor(this._settings, this._visitors);
        }

        #region 查询

        public override IEnumerable<T> Query<T>(IDbContext context, CommandSql commandSql)
        {
            var influx17xContext = context as Influx17xDbContext;
            var influx = influx17xContext.Influx;

            // 生成sql
            var sql = commandSql.ToInflux17xSql();

            // 开始结束时间
            if (influx17xContext.Start.HasValue
                && influx17xContext.End.HasValue)
            {
                var rangeSqls = CreateSql(
                        sql,
                        influx17xContext.Start.Value,
                        influx17xContext.End.Value
                    );

                var multiQuerySeries = influx.Client.MultiQueryAsync(
                        rangeSqls,
                        influx17xContext.Influx.GetDatabaseName()
                    )
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                var multiQueryReader = new Influx17xDataReader();
                foreach (var singleQuerySeries in multiQuerySeries)
                {
                    foreach (var serie in singleQuerySeries)
                    {
                        multiQueryReader.SetOptions(
                            serie.Columns,
                            serie.Values
                        );
                        while (multiQueryReader.Read())
                        {
                            yield return Mapper.ThrowsMap<T>(multiQueryReader);
                        }
                    }
                }

                yield break;
            }

            // 单表时间sql生成
            if (influx17xContext.Start.HasValue
                && !influx17xContext.End.HasValue)
            {
                sql = this.CreateSql(sql, influx17xContext.Start.Value);
            }


            {
                var singleQuerySeries = influx.Client.QueryAsync(
                    sql,
                    influx17xContext.Influx.GetDatabaseName()
                )
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

                var singleQueryReader = new Influx17xDataReader();
                foreach (var serie in singleQuerySeries)
                {
                    singleQueryReader.SetOptions(
                        serie.Columns,
                        serie.Values
                        );

                    while (singleQueryReader.Read())
                    {
                        yield return Mapper.ThrowsMap<T>(singleQueryReader);
                    }
                }
            }
        }

        public override IAsyncEnumerable<T> QueryAsync<T>(IDbContext context, CommandSql commandSql)
        {
            throw new System.NotImplementedException();
        }

        #endregion


        #region 读

        public override T Read<T>(IDbContext context, CommandSql<T> commandSql)
        {
            var enumerator = this.Query<T>(context, commandSql).GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public override async Task<T> ReadAsync<T>(IDbContext context, CommandSql<T> commandSql, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();

            var tmp = this.QueryAsync<T>(context, commandSql);

            var enumerator = tmp.GetAsyncEnumerator(cancellationToken);

            await enumerator.MoveNextAsync();

            return enumerator.Current;
        }

        #endregion


        #region 执行-未实现

        public override int Execute(IDbContext context, CommandSql commandSql)
        {
            throw new System.NotImplementedException();
        }

        public override Task<int> ExecuteAsync(IDbContext context, CommandSql commandSql, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region MyRegion

        /// <summary>
        /// 创建sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        protected virtual string CreateSql(string sql, DateTime start)
        {
            var sqlTemplate = GetSqlTemplate(sql, out string tableName);
            return string.Format(
                    sqlTemplate,
                    tableName,
                    start.ToString("yyyyMM")
                    );
        }

        /// <summary>
        /// 创建sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected virtual IEnumerable<string> CreateSql(string sql, DateTime start, DateTime end)
        {
            var sqlTemplate = GetSqlTemplate(sql, out string tableName);
            while (true)
            {
                if (start > end)
                {
                    break;
                }
                var newSql = string.Format(
                   sqlTemplate,
                   tableName,
                   start.ToString("yyyyMM")
                   );
                yield return newSql;
                start = start.AddMonths(1);

            }

            {
                var newSql = string.Format(
                   sqlTemplate,
                   tableName,
                   end.ToString("yyyyMM")
                   );
                yield return newSql;
            }
        }

        /// <summary>
        /// 生成sql模板
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="tableName">表名称</param>
        /// <returns>返回表模板</returns>
        protected virtual string GetSqlTemplate(string sql, out string tableName)
        {
            var match = Regex.Match(sql, " [fF][rR][oO][mM] \"(.*?)\" ");
            var sqlTemplate = sql.Replace(match.Value, " FROM \"{0}_{1}\" ");
            tableName = match.Groups[1].Value;

            return sqlTemplate;
        }


        #endregion
    }
}
