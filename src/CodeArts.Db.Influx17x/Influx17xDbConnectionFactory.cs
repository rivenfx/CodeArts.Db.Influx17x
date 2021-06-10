using System.Data;

namespace CodeArts.Db
{
    public class Influx17xDbConnectionFactory : IDbConnectionFactory
    {
        public const string Name = "Influx17x";

        public string ProviderName => Name;

        public double ConnectionHeartbeat { get; set; } = 5D;

        public int MaxPoolSize { set; get; } = 10;

        public IDbConnection Create(string connectionString)
        {
            return null;
        }
    }
}
