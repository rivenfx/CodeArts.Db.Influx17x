
using CodeArts.Db.Lts;

using System;
using System.Data;

namespace CodeArts.Db
{
    public class Influx17xAdpter : Influx17xDbConnectionFactory, IDbConnectionLtsAdapter
    {
        /// <summary>
        /// 矫正配置。
        /// </summary>
        public virtual ISQLCorrectSettings Settings => Singleton<Influx17xCorrectSettings>.Instance;

        private CustomVisitorList visitters;

        /// <summary>
        /// 格式化。
        /// </summary>
        public ICustomVisitorList Visitors => visitters ?? (visitters = new CustomVisitorList());
    }
}
