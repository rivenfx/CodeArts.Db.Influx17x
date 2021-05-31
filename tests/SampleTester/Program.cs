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
            QueryData(connectionConfig);

        }


        static void InsertData(SampleInfluxClient influxDbClient)
        {
            var mySensor = new MySensor()
            {
                SensorId = "1",
                DataType = "number",
                Label = "测试传感器",
                Timestamp = DateTime.UtcNow,
                Tmp = "临时字段",
                Value = 1
            };

            var writeRes = influxDbClient.InsertAsync(
                   mySensor,
                   timestampAddToTableName: true
                   )
                   .ConfigureAwait(false)
                   .GetAwaiter()
                   .GetResult();
        }

        static void QueryData(Influx17xConnectionConfig connectionConfig)
        {
            var portionDate = DateTime.UtcNow;
            var extenstionTableName = "number";

            var tableName = Influx17xEntityMaper.BasicInstance
                .GetTableName<MySensor>(
                    portionDate, 
                    extenstionTableName, 
                    true
                );
            var query = Influx17xHelper
                .CreateQuery<MySensor>(connectionConfig, tableName)
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
}

