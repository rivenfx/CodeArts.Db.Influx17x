using CodeArts.Db.Lts;

using System;

namespace CodeArts.Db
{
    public class Influx17xAdpter : IDbConnectionLtsAdapter
    {
        public const string Name = "Influx17x";

        /// <summary> 
        /// 适配器名称。 
        /// </summary>
        public string ProviderName => Name;

        /// <summary>
        /// 矫正配置。
        /// </summary>
        public virtual ISQLCorrectSettings Settings => Singleton<Influx17xCorrectSettings>.Instance;

        private CustomVisitorList visitters;

        /// <summary>
        /// 格式化。
        /// </summary>
        public ICustomVisitorList Visitors => visitters ?? (visitters = new CustomVisitorList());

        public double ConnectionHeartbeat => throw new NotImplementedException();

        public int MaxPoolSize => 0;

        public System.Data.IDbConnection Create(string connectionString)
        {
            return null;
        }
    }
}
