# Riven.CodeArts.Db.Influx17x
LINQ queries are provided for influxdb version 1.7+


## LICENSES
![GitHub](https://img.shields.io/github/license/rivenfx/Modular?color=brightgreen)
[![Badge](https://img.shields.io/badge/link-996.icu-%23FF4D5B.svg?style=flat-square)](https://996.icu/#/zh_CN)
[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg?style=flat-square)](https://github.com/996icu/996.ICU/blob/master/LICENSE)

Please note: once the use of the open source projects as well as the reference for the project or containing the project code for violating labor laws (including but not limited the illegal layoffs, overtime labor, child labor, etc.) in any legal action against the project, the author has the right to punish the project fee, or directly are not allowed to use any contains the source code of this project!

## Build Status

[![Build Status](https://dev.azure.com/rivenfx/RivenFx/_apis/build/status/rivenfx.CodeArts.Db.Influx17x?branchName=master)](https://dev.azure.com/rivenfx/RivenFx/_build/latest?definitionId=8&branchName=master)

## Nuget Packages

|Package|Status|Downloads|
|:------|:-----:|:-----:|
|Riven.CodeArts.Db.Influx17x|[![NuGet version](https://img.shields.io/nuget/v/Riven.CodeArts.Db.Influx17x?color=brightgreen)](https://www.nuget.org/packages/Riven.CodeArts.Db.Influx17x/)|[![Nuget](https://img.shields.io/nuget/dt/Riven.CodeArts.Db.Influx17x?color=brightgreen)](https://www.nuget.org/packages/Riven.CodeArts.Db.Influx17x/)|


## Quick start
1. Install Nuget Package `Riven.CodeArts.Db.Influx17x`

2. using namespaces
```c#
using System;
using System.Linq;
using System.Collections.Generic;
using CodeArts;
using CodeArts.Db;
```

3. Initialize
```c#
// 连接参数
var influxOptions = new InfluxOptions(
                    "http://192.168.1.116:8086", // influx server
                    "test2", // database name
                    userName: string.Empty, // username
                    password: string.Empty // password
              );

var influxDbClient = influxOptions.CreateSampleInfluxClient();
var connectionConfig = new Influx17xConnectionConfig(
                   influxDbClient
);

// 初始化 CodeArts
CodeArtsHelper.InitializeBasic();

// 初始化 CodeArts.Db.Influx17x
Influx17xHelper.InitializeCodeArts();
Influx17xHelper.InitializeDefaultConnectionConfig(connectionConfig);

```

4. Define entity types
```c#

    public class MySensor
    {
        // tag column, require [Influx17xColumn(Influx17xColumnType.Tag)] 
        [Influx17xColumn(Influx17xColumnType.Tag)]
        public string SensorId { get; set; }

        // Timestamp column,require [Naming("time")] and  [Influx17xColumn(Influx17xColumnType.Timestamp)]
        // Only one column is allowed
        [Naming("time")]
        [Influx17xColumn(Influx17xColumnType.Timestamp)]
        public DateTime Timestamp { get; set; }

        // table extension name column,require [Influx17xColumn(Influx17xColumnType.TableExtensionName)]
        // Only one column is allowed
        [Influx17xColumn(Influx17xColumnType.TableExtensionName)]
        public int DataType { get; set; }

        // Ignore mapping columns [Ignore]
        [Ignore]
        public string Tmp { get; set; }

        public float Value { get; set; }

        public string Label { get; set; }
    }
```

5. Write datas
```c#
var data=new MySensor();
var writeRes = influxDbClient.InsertAsync(
                data,
                timestampAddToTableName: true // tableName true: MySensor202001 false: MySensor
                )
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

var dataList = new List<MySensor>();        
writeRes = influxDbClient.InsertAsync(
                dataList,
                timestampAddToTableName: false // tableName true: MySensor202001 false: MySensor
                )
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();                
```


6. Query datas
```c#
var query = Influx17xHelper.CreateQuery<MySensor>(connectionConfig,null)
                .Where(o => o.Value > -1)
                //.Skip(0)
                //.Take(10)
                ;
var res = query.ToList();                
```

## Demos

> TODO


## Q&A

If you have any questions, you can go to  [Issues](https://github.com/rivenfx/CodeArts.Db.Influx17x/issues) to ask them.


## Reference project

> This project directly or indirectly refers to the following items

- [codearts](https://github.com/tinylit/codearts)
- [InfluxData.Net](https://github.com/tihomir-kit/InfluxData.Net)
- [influxdb-client-csharp](https://github.com/influxdata/influxdb-client-csharp)


## Stargazers over time

[![Stargazers over time](https://starchart.cc/rivenfx/CodeArts.Db.Influx17x.svg)](https://starchart.cc/rivenfx/CodeArts.Db.Influx17x)
