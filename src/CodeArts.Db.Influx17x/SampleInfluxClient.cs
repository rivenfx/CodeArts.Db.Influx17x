using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CodeArts.Db
{
    public class SampleInfluxClient : InfluxDbClient
    {
        public readonly string _databaseName;

        public SampleInfluxClient(string endpointUri, string databaseName, string username, string password, InfluxDbVersion influxVersion, QueryLocation queryLocation = QueryLocation.FormData, HttpClient httpClient = null, bool throwOnWarning = false)
            : base(endpointUri, username, password, influxVersion, queryLocation, httpClient, throwOnWarning)
        {
            this._databaseName = databaseName;
        }
    }


    public static class SampleInfluxClientExtensions
    {
        public static string GetDatabaseName(this IInfluxDbClient influxClient)
        {
            if (influxClient is SampleInfluxClient client)
            {
                return client._databaseName;
            }

            return null;
        }
    }
}
