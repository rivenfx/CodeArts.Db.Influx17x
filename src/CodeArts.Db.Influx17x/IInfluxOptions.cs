
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CodeArts.Db
{
    public interface IInfluxOptions
    {
        /// <summary>
        /// 服务端
        /// </summary>
        string Server { get; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        string DatabaseName { get; }
        /// <summary>
        /// 账号
        /// </summary>
        string UserName { get; }
        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; }

        /// <summary>
        /// 实体测点转换器
        /// </summary>
        Influx17xEntityMaper Mapper { get; }
    }


    public class InfluxOptions : IInfluxOptions
    {
        public InfluxOptions(string server, string databaseName, string userName = "", string password = "", Influx17xEntityMaper mapper = null)
        {
            Server = server;
            DatabaseName = databaseName;
            UserName = userName;
            Password = password;
        }

        public string Server { get; }
        public string DatabaseName { get; }
        public string UserName { get; }
        public string Password { get; }

        public Influx17xEntityMaper Mapper { get; }
    }


    public static class IInfluxOptionsExtenstions
    {
        public static SampleInfluxClient CreateSampleInfluxClient(this IInfluxOptions options, HttpClient httpClient = null, Influx17xEntityMaper mapper = null)
        {
            return new SampleInfluxClient(
                options.Server,
                options.DatabaseName,
                options.UserName,
                options.Password,
                influxVersion: InfluxDbVersion.v_1_3,
                httpClient: httpClient ?? new HttpClient(),
                mapper: mapper
                );
        }
    }
}
