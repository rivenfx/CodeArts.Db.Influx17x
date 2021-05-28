using InfluxData.Net.InfluxDb.Models;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeArts.Db
{
    /// <summary>
    /// influx17 的实体转换器
    /// </summary>
    public class Influx17xEntityMaper
    {
        static string DateTimeTypeStr = typeof(DateTime).FullName;

        private IDictionary<string, string> TABLE_CACHE = new ConcurrentDictionary<string, string>();
        private IDictionary<string, Influx17xColumnInfo[]> PROP_CACHE = new ConcurrentDictionary<string, Influx17xColumnInfo[]>();


        /// <summary>
        /// 类型转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual IEnumerable<Point> ToPoint<T>(IEnumerable<T> entitys)
            where T : class, new()
        {
            foreach (var entity in entitys)
            {
                yield return ToPoint(entity);
            }
        }

        public virtual Point ToPoint<T>(T entity)
             where T : class, new()
        {
            if (entity is Point point)
            {
                return point;
            }

            // 创建缓存
            var entityType = entity.GetType();
            CacheEntityClass(entityType);

            // 取缓存
            var tableName = TABLE_CACHE[entityType.FullName];
            var tableProps = PROP_CACHE[entityType.FullName];

            // 创建point
            point = new Point()
            {
                Name = tableName,
                Tags = new Dictionary<string, object>(),
                Fields = new Dictionary<string, object>(),
            };

            // 构建point
            foreach (var prop in tableProps)
            {
                var value = prop.Property.GetValue(entity);
                if (value == null)
                {
                    continue;
                }

                switch (prop.ColumnType)
                {
                    case Influx17xColumnType.Tag:
                        {
                            if (value is string str)
                            {
                                point.Tags[prop.ColumnName] = str;
                            }
                            else
                            {
                                point.Tags[prop.ColumnName] = value.ToString();
                            }
                        }
                        break;
                    case Influx17xColumnType.Timestamp:
                        {
                            if (value is DateTime date)
                            {
                                point.Timestamp = date;
                            }
                        }
                        break;
                    case Influx17xColumnType.Field:
                        {
                            if (value is bool b)
                            {
                                point.Fields[prop.ColumnName] = b;
                            }
                            else if (value is double d)
                            {
                                point.Fields[prop.ColumnName] = d;
                            }
                            else if (value is float f)
                            {
                                point.Fields[prop.ColumnName] = f;
                            }
                            else if (value is decimal dec)
                            {
                                point.Fields[prop.ColumnName] = dec;
                            }
                            else if (value is long lng)
                            {
                                point.Fields[prop.ColumnName] = lng;
                            }
                            else if (value is ulong ulng)
                            {
                                point.Fields[prop.ColumnName] = ulng;
                            }
                            else if (value is int i)
                            {
                                point.Fields[prop.ColumnName] = i;
                            }
                            else if (value is byte bt)
                            {
                                point.Fields[prop.ColumnName] = bt;
                            }
                            else if (value is sbyte sb)
                            {
                                point.Fields[prop.ColumnName] = sb;
                            }
                            else if (value is short sh)
                            {
                                point.Fields[prop.ColumnName] = sh;
                            }
                            else if (value is uint ui)
                            {
                                point.Fields[prop.ColumnName] = ui;
                            }
                            else if (value is ushort us)
                            {
                                point.Fields[prop.ColumnName] = us;
                            }
                            else
                            {
                                point.Fields[prop.ColumnName] = value.ToString();
                            }
                        }
                        break;
                }
            }

            return point;
        }

        /// <summary>
        /// 缓存实体类型
        /// </summary>
        /// <param name="entityType"></param>
        public virtual void CacheEntityClass(Type entityType)
        {


            if (PROP_CACHE.ContainsKey(entityType.FullName))
            {
                return;
            }

            // 表名称
            TABLE_CACHE[entityType.FullName] = entityType.Name.ToLower();
            var tableNaming = entityType.GetCustomAttribute(typeof(NamingAttribute)) as NamingAttribute;
            if (tableNaming != null
                && !string.IsNullOrWhiteSpace(tableNaming.Name))
            {
                TABLE_CACHE[entityType.FullName] = tableNaming.Name
                    .Trim()
                    .ToLower();
            }

            // 时间戳类型列计数
            var timestamp = 0;
            // 列处理
            PROP_CACHE[entityType.FullName] = entityType.GetProperties()
                .Select(property =>
                {
                    var info = new Influx17xColumnInfo();
                    info.ColumnType = Influx17xColumnType.Field;
                    info.Property = property;

                    // 列名
                    var naming = property.GetCustomAttribute(typeof(NamingAttribute)) as NamingAttribute;
                    if (naming != null && !string.IsNullOrWhiteSpace(naming.Name))
                    {
                        info.ColumnName = naming.Name.Trim().ToLower();
                    }
                    else
                    {
                        info.ColumnName = property.Name.ToLower();
                    }

                    // 列类型
                    var columnType = property.GetCustomAttribute(typeof(Influx17xColumnAttribute)) as Influx17xColumnAttribute;
                    if (columnType != null)
                    {
                        info.ColumnType = columnType.Type;
                    }

                    // 时间戳处理
                    switch (info.ColumnType)
                    {
                        case Influx17xColumnType.Timestamp: // 时间戳
                            {
                                // 时间戳列类型必须为 DateTime
                                if (info.Property.PropertyType
                                .FullName != DateTimeTypeStr)
                                {
                                    throw new ArgumentException(
                                        $"Timestamp 列只支持 {DateTimeTypeStr} 类型"
                                        );
                                }
                                // 只允许存在一个时间戳列
                                if (timestamp > 1)
                                {
                                    throw new ArgumentException(
                                        $"存在多个 Timestamp 列"
                                        );
                                }

                                timestamp++;
                            }
                            break;
                    }
                    return info;

                })
                .Where(propertyInfo => propertyInfo.ColumnName != null)
                .ToArray();
        }
    }
}
