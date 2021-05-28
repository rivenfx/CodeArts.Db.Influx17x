using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace CodeArts.Db
{
    public class SampleInfluxClient : InfluxDbClient
    {
        protected readonly string _databaseName;
        protected readonly Influx17xEntityMaper _mapper;

        public SampleInfluxClient(string endpointUri, string databaseName, string username, string password, InfluxDbVersion influxVersion, QueryLocation queryLocation = QueryLocation.FormData, HttpClient httpClient = null, bool throwOnWarning = false, Influx17xEntityMaper mapper = null)
            : base(endpointUri, username, password, influxVersion, queryLocation, httpClient, throwOnWarning)
        {
            this._databaseName = databaseName;

            if (true)
            {

            }
            _mapper = mapper ?? Influx17xEntityMaper.BasicInstance;
        }


        public virtual string DatabaseName => this._databaseName;
        public virtual Influx17xEntityMaper Mapper => this._mapper;



        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="influxClient">客户端</param>
        /// <param name="data">数据</param>
        /// <param name="retentionPolicy">策略, 默认 autogen</param>
        /// <param name="precision">精度,默认 ms</param>
        /// <param name="mapper">实体转Point转换器</param>
        /// <returns></returns>
        public Task<IInfluxDataApiResponse> InsertAsync<T>(T data, string retentionPolicy = "autogen", string precision = "ms")
            where T : class, new()
        {
            if (data is Point point)
            {
                return this.Client.WriteAsync(
                      point,
                      this.DatabaseName,
                      retentionPolicy,
                      precision
                      );
            }

            if (data is IEnumerable<T> points)
            {
                return this.BatchInsertAsync(points, retentionPolicy, precision);
            }

            return this.Client.WriteAsync(
                      this.Mapper.ToPoint<T>(data),
                      this.DatabaseName,
                      retentionPolicy,
                      precision
                      );
        }

        /// <summary>
        /// 新增数据-批量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="influxClient">客户端</param>
        /// <param name="datas">数据</param>
        /// <param name="retentionPolicy">策略, 默认 autogen</param>
        /// <param name="precision">精度,默认 ms</param>
        /// <returns></returns>
        public virtual Task<IInfluxDataApiResponse> BatchInsertAsync<T>(IEnumerable<T> datas, string retentionPolicy = "autogen", string precision = "ms")
             where T : class, new()
        {
            if (datas is IEnumerable<Point> points)
            {
                return this.Client.WriteAsync(
                      points,
                      this.DatabaseName,
                      retentionPolicy,
                      precision
                      );
            }

            points = this.Mapper.ToPoints<T>(datas);
            var tmp = points.ToList();

            return this.Client.WriteAsync(
                      tmp,
                      this.DatabaseName,
                      retentionPolicy,
                      precision
                      );
        }

        public virtual Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(IEnumerable<string> queries, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            return this.Client.MultiQueryAsync(
                 queries,
                 this.DatabaseName,
                 epochFormat,
                 chunkSize
                 );
        }
        public virtual Task<IEnumerable<Serie>> QueryAsync(string query, string epochFormat = null, long? chunkSize = null)
        {
            return this.Client.QueryAsync(
                query,
                this.DatabaseName,
                epochFormat,
                chunkSize
                );
        }

        public virtual Task<IEnumerable<Serie>> QueryAsync(IEnumerable<string> queries, string epochFormat = null, long? chunkSize = null)
        {
            return this.Client.QueryAsync(
               queries,
               this.DatabaseName,
               epochFormat,
               chunkSize
               );
        }

        public virtual Task<IEnumerable<Serie>> QueryAsync(string queryTemplate, object parameters, string epochFormat = null, long? chunkSize = null)
        {
            return this.Client.QueryAsync(
               queryTemplate,
               parameters,
               this.DatabaseName,
               epochFormat,
               chunkSize
               );
        }
    }


    public static class SampleInfluxClientExtensions
    {
        public static string GetDatabaseName(this IInfluxDbClient influxClient)
        {
            if (influxClient is SampleInfluxClient client)
            {
                return client.DatabaseName;
            }

            return null;
        }
    }
}
