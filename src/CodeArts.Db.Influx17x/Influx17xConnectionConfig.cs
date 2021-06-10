using InfluxData.Net.InfluxDb;

namespace CodeArts.Db
{
    public class Influx17xConnectionConfig : IConfigable<Influx17xConnectionConfig>, IReadOnlyConnectionConfig
    {
        public Influx17xConnectionConfig(SampleInfluxClient influxClient)
        {
            this.ProviderName = Influx17xDbConnectionFactory.Name;
            this.InfluxClient = influxClient;
            this.ConnectionString = influxClient.EndpointUri;
        }

        public virtual string Name { get; set; }

        public virtual string ProviderName { get; set; }

        public virtual string ConnectionString { get; set; }

        public virtual SampleInfluxClient InfluxClient { get; protected set; }

        public void SaveChanges(Influx17xConnectionConfig changedValue)
        {
            if (changedValue is null)
            {
                return;
            }
            this.Name = changedValue.Name;
            this.ProviderName = changedValue.ProviderName;
            this.ConnectionString = changedValue.ConnectionString;
            this.InfluxClient = changedValue.InfluxClient;
        }
    }
}
