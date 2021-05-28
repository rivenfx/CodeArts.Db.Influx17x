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

            var influxDbClient = influxOptions.CreateSampleInfluxClient();
            var connectionConfig = new Influx17xConnectionConfig(
                   influxDbClient
               );

            // 初始化 CodeArts
            CodeArtsHelper.InitializeBasic();

            // 初始化 CodeArts.Db.Influx17x
            Influx17xHelper.InitializeDefaultConnectionConfig(connectionConfig);
            Influx17xHelper.InitializeCodeArts();


            // 新增数据
            InsertData(influxDbClient);
            // 查询数据
            QueryData1(influxDbClient);
            QueryData2(influxDbClient);

        }


        static void InsertData(SampleInfluxClient client)
        {
            var writeRes = client.InsertAsync(new MySensor()
            {
                SensorId = "none",
                Deployment = "point none",
                Value = 1,
                Label = "tmp",
                Timestamp = DateTime.Now
            }).ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();

            var start1 = new DateTime(
                           2021,
                           01,
                           01,
                           01,
                           01,
                           01,
                           DateTimeKind.Utc
                           );
            var dataList = new List<MySensor>();
            for (int i = 0; i < 100; i++)
            {
                var sensor = new MySensor()
                {
                    SensorId = (i + 100).ToString(),
                    Deployment = $"point {(i + 100)}",
                    Value = i,
                    Label = "tmp",
                    Timestamp = start1
                };

                dataList.Add(sensor);

                start1 = start1.AddDays(1);
            }

            writeRes = client.BatchInsertAsync(
                dataList,
                timestampAddToTableName: true
                )
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        static void QueryData1(SampleInfluxClient client)
        {
            var query = Influx17xHelper
                .CreateDeaultQuery<MySensor>()
                //.CreateDeaultQuery<MySensor>(DateTime.Parse("2021-01-01"))
                .Where(o => o.Value > -1)
                //.Skip(0)
                //.Take(10)
                ;


            // 获取结果
            var res = query.ToList();

            // 打印sql
            Console.WriteLine(query.ToString());
        }

        static void QueryData2(SampleInfluxClient client)
        {
            var query = Influx17xHelper
                .CreateDeaultQuery<MySensor>(DateTime.Parse("2021-01-01"))
                .Where(o => o.Value > -1)
                //.Skip(0)
                //.Take(10)
                ;


            // 获取结果
            var res = query.ToList();

            // 打印sql
            Console.WriteLine(query.ToString());
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

        public string Label { get; set; }

        [Naming("time")]
        [Influx17xColumn(Influx17xColumnType.Timestamp)]
        public DateTime Timestamp { get; set; }
    }
}

