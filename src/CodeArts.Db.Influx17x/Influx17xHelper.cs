using CodeArts.Casting;
using CodeArts.Db.Lts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArts.Db
{
    public static class Influx17xHelper
    {
        public static Influx17xConnectionConfig ConnectionConfig { get; private set; }

        /// <summary>
        /// 初始化 Influx17xHelper
        /// </summary>
        /// <param name="connectionConfig"></param>
        public static void Initialize(Influx17xConnectionConfig defaultConnectionConfig)
        {
            if (ConnectionConfig != null)
            {
                return;
            }
            if (defaultConnectionConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(defaultConnectionConfig),
                    "输入参数不能为空"
                    );
            }


            ConnectionConfig = defaultConnectionConfig;

            //
            DbConnectionManager.RegisterAdapter(new Influx17xAdpter());
            DbConnectionManager.RegisterProvider<Influx17xCodeArtsProvider>(
                Influx17xAdpter.Name
                );
        }


        /// <summary>
        /// 创建一个查询器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionConfig">配置信息</param>
        /// <param name="tableName">表名</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IQueryable<T> CreateQuery<T>(Influx17xConnectionConfig connectionConfig, string tableName, DateTime? start = null, DateTime? end = null)
        {
            // 参数校验
            if (connectionConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(connectionConfig),
                    "输入参数不能为空"
                    );
            }

            // 如果开始为空,结束不为空,则将结束值给开始,并将结束置空
            if (!start.HasValue && end.HasValue)
            {
                start = end;
                end = null;
            }

            // 交换开始结束时间范围
            if (start.HasValue
                && end.HasValue)
            {
                if (start > end)
                {
                    var tmp = start;
                    end = start;
                    start = end;
                }
                // 开始和结束相等,结束置空
                else if (start == end)
                {
                    end = null;
                }
            }

            #region 统一处理时间格式

            if (start.HasValue)
            {
                start = new DateTime(
                    start.Value.Year,
                    start.Value.Month,
                    01, 00, 00, 00,
                    DateTimeKind.Utc
                    );
            }
            if (end.HasValue)
            {
                end = new DateTime(
                    end.Value.Year,
                    end.Value.Month,
                    01, 00, 00, 00,
                    DateTimeKind.Utc
                    );
            }

            #endregion


            var query = new Influx17xQueryable<T>(connectionConfig, start, end);
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                return query.From(o => tableName);
            }
            return query;
        }


        /// <summary>
        /// 创建默认配置的查询器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> CreateDeaultQuery<T>()
        {
            return CreateDeaultQuery<T>(null);
        }

        /// <summary>
        /// 创建默认配置的查询器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partion">分表时间</param>
        /// <returns></returns>
        public static IQueryable<T> CreateDeaultQuery<T>(DateTime? partion)
        {
            return CreateDeaultQuery<T>(partion, null);
        }

        /// <summary>
        /// 创建默认配置的查询器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名称</param>
        /// <param name="partion">分表时间</param>
        /// <returns></returns>
        public static IQueryable<T> CreateDeaultQuery<T>(string tableName, DateTime? partion)
        {
            return CreateDeaultQuery<T>(tableName, partion, null);
        }



        /// <summary>
        /// 创建默认配置的查询器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名称</param>
        /// <param name="start">分表开始时间</param>
        /// <param name="end">分表结束时间</param>
        /// <returns>查询器</returns>
        static IQueryable<T> CreateDeaultQuery<T>(string tableName, DateTime? start, DateTime? end)
        {
            if (ConnectionConfig == null)
            {
                throw new ArgumentException(
                    $"Influx17xHelper 未找到默认配置, 请初始化之后使用!"
                    );
            }




            return CreateQuery<T>(ConnectionConfig, tableName, start, end);
        }

        /// <summary>
        /// 创建默认配置的查询器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start">分表开始时间</param>
        /// <param name="end">分表结束时间</param>
        /// <returns></returns>
        static IQueryable<T> CreateDeaultQuery<T>(DateTime? start, DateTime? end)
        {
            return CreateDeaultQuery<T>(null, start, end);
        }
    }
}
