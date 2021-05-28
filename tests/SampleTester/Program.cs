using CodeArts;
using CodeArts.Db;
using CodeArts.Db.Lts;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;
using CodeArts.Casting;
using CodeArts.Casting.Implements;
using System.Reflection;

namespace SampleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var influxOptions = new InfluxOptions(
                    "http://192.168.1.116:8086",
                    "test2",
                    userName: string.Empty,
                    password: string.Empty
              );

            var influxDbClient = influxOptions.CreateSampleInfluxClient(
                mapper: Influx17xEntityMaper.TimestampInstance
                );
            var connectionConfig = new Influx17xConnectionConfig(
                   influxDbClient
               );

            //var start1 = new DateTime(
            //               2021,
            //               01,
            //               01,
            //               01,
            //               01,
            //               01,
            //               DateTimeKind.Utc
            //               );
            //var dataList = new List<MySensor>();
            //for (int i = 0; i < 100; i++)
            //{
            //    //var pointData = PointData
            //    //    .Measurement($"mysensor_{start1.ToString("yyyyMM")}")
            //    //    .Tag("sensor_id", i.ToString())
            //    //    .Field("deployment", $"point {i}")
            //    //    .Field("data", i)
            //    //    .TimeStamp(start1);
            //    //;
            //    var sensor = new MySensor()
            //    {
            //        SensorId = i.ToString(),
            //        Deployment = $"point {i}",
            //        Value = i,
            //        Timestamp = start1
            //    };

            //    dataList.Add(sensor);

            //    start1 = start1.AddDays(1);
            //}
            //var writeRes = influxDbClient.BatchInsertAsync(dataList)
            //    .ConfigureAwait(false)
            //    .GetAwaiter()
            //    .GetResult();


            CodeArtsHelper.InitializeBasic();

            Influx17xHelper.InitializeDefaultConnectionConfig(connectionConfig);
            Influx17xHelper.InitializeCodeArts();

            var query = Influx17xHelper
                .CreateDeaultQuery<MySensor>(DateTime.Parse("2021-01-01"))
                .Where(o => o.Value > -1)
                //.Skip(0)
                //.Take(10)
                ;

            //influxDbClient.Client.WriteAsync()

            Console.WriteLine(query.ToString());
            var res = query.ToList();
        }
    }


    [Naming("mysensor")]
    public class MySensor
    {
        [Naming("sensor_id")]
        [Influx17xColumn(Influx17xColumnType.Tag)]
        public string SensorId { get; set; }

        [Naming("deployment")]
        [Influx17xColumn(Influx17xColumnType.Tag)]
        public string Deployment { get; set; }

        [Naming("data")]
        public float Value { get; set; }

        [Naming("time")]
        [Influx17xColumn(Influx17xColumnType.Timestamp)]
        public DateTime Timestamp { get; set; }
    }
}

