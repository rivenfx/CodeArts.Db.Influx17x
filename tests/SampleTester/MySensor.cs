using CodeArts;
using CodeArts.Db;

using System;

namespace SampleTester
{
    public class MySensor
    {
        // [Influx17xColumn(Influx17xColumnType.Tag)] tag类型列
        [Influx17xColumn(Influx17xColumnType.Tag)]
        public string SensorId { get; set; }

        // 时间戳列,必须标记为 [Naming("time")] 和 [Influx17xColumn(Influx17xColumnType.Timestamp)]
        [Naming("time")]
        [Influx17xColumn(Influx17xColumnType.Timestamp)]
        public DateTime Timestamp { get; set; }

        // 表扩展名称
        [Influx17xColumn(Influx17xColumnType.TableExtensionName)]
        public string DataType { get; set; }

        // 忽略映射的列
        [Ignore]
        public string Tmp { get; set; }

        public float Value { get; set; }

        public string Label { get; set; }
    }
}

