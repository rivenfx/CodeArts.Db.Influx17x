using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArts.Db
{
    public static class Influx17xClientExtenstions
    {
        static Influx17xEntityMaper _defaultMaper;
        static Influx17xClientExtenstions()
        {
            _defaultMaper = new Influx17xEntityMaper();
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="influxClient">客户端</param>
        /// <param name="point">数据</param>
        /// <param name="retentionPolicy">策略, 默认 autogen</param>
        /// <param name="precision">精度,默认 ms</param>
        /// <param name="mapper">实体转Point转换器</param>
        /// <returns></returns>
        public static Task<IInfluxDataApiResponse> WriteAsync<T>(this IInfluxDbClient influxClient, T point, string retentionPolicy = "autogen", string precision = "ms", Influx17xEntityMaper mapper = null)
            where T : class, new()
        {
            var sampleClient = influxClient as SampleInfluxClient;
            if (sampleClient == null)
            {
                throw new ArgumentException(
                    $"InfluxDbClient 实例类型不是 {typeof(SampleInfluxClient).FullName}",
                    nameof(influxClient)
                    );
            }


            if (point is Point input)
            {
                return sampleClient.Client.WriteAsync(
                      input,
                      sampleClient.GetDatabaseName(),
                      retentionPolicy,
                      precision
                      );
            }

            if (mapper == null)
            {
                mapper = _defaultMaper;
            }
            return sampleClient.Client.WriteAsync(
                       mapper.ToPoint(point),
                      sampleClient.GetDatabaseName(),
                      retentionPolicy,
                      precision
                      );
        }

        /// <summary>
        /// 新增数据-批量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="influxClient">客户端</param>
        /// <param name="points">数据</param>
        /// <param name="retentionPolicy">策略, 默认 autogen</param>
        /// <param name="precision">精度,默认 ms</param>
        /// <param name="mapper">实体转Point转换器</param>
        /// <returns></returns>
        public static Task<IInfluxDataApiResponse> WriteAsync<T>(this IInfluxDbClient influxClient, IEnumerable<T> points, string retentionPolicy = "autogen", string precision = "ms", Influx17xEntityMaper mapper = null)
             where T : class, new()
        {
            var sampleClient = influxClient as SampleInfluxClient;
            if (sampleClient == null)
            {
                throw new ArgumentException(
                    $"InfluxDbClient 实例类型不是 {typeof(SampleInfluxClient).FullName}",
                    nameof(influxClient)
                    );
            }

            if (points is IEnumerable<Point> input)
            {
                return sampleClient.Client.WriteAsync(
                      input,
                      sampleClient.GetDatabaseName(),
                      retentionPolicy,
                      precision
                      );
            }

            if (mapper == null)
            {
                mapper = _defaultMaper;
            }
            return sampleClient.Client.WriteAsync(
                      mapper.ToPoint(points),
                      sampleClient.GetDatabaseName(),
                      retentionPolicy,
                      precision
                      );
        }
    }



}
