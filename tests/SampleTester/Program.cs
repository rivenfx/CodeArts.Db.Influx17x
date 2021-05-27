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

namespace SampleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var influxOptions = new InfluxOptions(
                    "http://192.168.1.111:8086",
                    "testdb",
                    userName: string.Empty,
                    password: string.Empty
              );

            var influxDbClient = influxOptions.CreateInfluxClient();
            var connectionConfig = new Influx17xConnectionConfig(
                   influxDbClient
               );

            CodeArtsHelper.InitializeBasic();
            Influx17xHelper.Initialize(connectionConfig);

            var query = Influx17xHelper
                .CreateDeaultQuery<MySensor>(DateTime.Parse("2021-01-01"))
                .Where(o => o.Value > -1)
                //.Skip(0)
                //.Take(10)
                ;
            Console.WriteLine(query.ToString());
            var res = query.ToList();
        }
    }


    [Naming("mysensor")]
    public class MySensor
    {
        [Naming("sensor_id")]
        public string SensorId { get; set; }

        [Naming("deployment")]
        public string Deployment { get; set; }

        [Naming("data")]
        public float Value { get; set; }

        [Naming("time")]
        public DateTime Timestamp { get; set; }
    }
}

