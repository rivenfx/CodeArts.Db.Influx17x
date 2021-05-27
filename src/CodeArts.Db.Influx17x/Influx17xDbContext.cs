using CodeArts.Db.Lts;

using InfluxData.Net.InfluxDb;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CodeArts.Db
{
    public class Influx17xDbContext : DbContext
    {
        protected readonly Influx17xConnectionConfig _connectionConfig;
        public Influx17xDbContext(Influx17xConnectionConfig connectionConfig, DateTime? start, DateTime? end)
            : base(connectionConfig)
        {
            _connectionConfig = connectionConfig;

            Start = start;
            End = end;
        }

        public virtual DateTime? Start { get; protected set; }

        public virtual DateTime? End { get; protected set; }

        public virtual IInfluxDbClient Influx => this._connectionConfig.InfluxClient;

        public virtual IDbConnectionLtsAdapter DatabaseAdapter => this.DbAdapter;

        public virtual IDbRepositoryProvider QueryProvider => this.DbProvider;

        public virtual IDbRepositoryExecuter Executer => this.DbExecuter;


        protected override CommandSql<T> CreateReadCommandSql<T>(Expression expression)
        {
            return base.CreateReadCommandSql<T>(expression);
        }
    }

}
