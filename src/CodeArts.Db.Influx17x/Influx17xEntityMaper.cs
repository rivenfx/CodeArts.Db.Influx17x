using InfluxData.Net.InfluxDb.Models;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeArts.Db
{
    /// <summary>
    /// influx1.x 基本的映射器
    /// </summary>
    public class Influx17xEntityMaper
    {
        protected static string DateTimeTypeStr = typeof(DateTime).FullName;

        protected IDictionary<string, string> TABLE_CACHE = new ConcurrentDictionary<string, string>();
        protected IDictionary<string, Influx17xColumnInfo[]> PROP_CACHE = new ConcurrentDictionary<string, Influx17xColumnInfo[]>();

        public static Influx17xEntityMaper BasicInstance { get; } = new Influx17xEntityMaper();

        /// <summary>
        /// 转Point
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="tableExtenstionName">表扩展名称,会追加到基本表名后: basci_tableExtenstionName</param>
        /// <param name="timestampAddToTableName">时间追加到表名</param>
        /// <returns></returns>
        public virtual IEnumerable<Point> ToPoints<T>(IEnumerable<T> entitys, string tableExtenstionName, bool timestampAddToTableName = false)
            where T : class, new()
        {
            foreach (var entity in entitys)
            {
                yield return ToPoint<T>(entity, tableExtenstionName, timestampAddToTableName);
            }
        }

        /// <summary>
        /// 转Point
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tableExtenstionName">表扩展名称,会追加到基本表名后: basci_tableExtenstionName</param>
        /// <param name="timestampAddToTableName">时间追加到表名</param>
        /// <returns></returns>
        public virtual Point ToPoint<T>(T entity, string tableExtenstionName, bool timestampAddToTableName = false)
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
                    case Influx17xColumnType.TableExtensionName:
                        {
                            // 扩展名不为空,跳过赋新值
                            if (!string.IsNullOrWhiteSpace(tableExtenstionName))
                            {
                                continue;
                            }

                            if (value is string s)
                            {
                                tableExtenstionName = s.Trim();
                            }
                            else
                            {
                                tableExtenstionName = value.ToString().Trim();
                            }
                            point.Fields[prop.ColumnName]
                                = tableExtenstionName;
                        }
                        break;
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

            // 如果没有时间戳
            if (!point.Timestamp.HasValue)
            {
                point.Timestamp = DateTime.UtcNow;
            }

            // 表名称
            point.Name = GetTableName(
                tableName,
                point.Timestamp,
                tableExtenstionName,
                timestampAddToTableName
                );

            return point;
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timestamp">数据时间</param>
        /// <param name="tableExtenstionName">表扩展名称,会追加到基本表名后: basci_tableExtenstionName</param>
        /// <param name="timestampAddToTableName">时间追加到表名</param>
        /// <returns>表名</returns>
        public virtual string GetTableName<T>(DateTime? timestamp, string tableExtenstionName, bool timestampAddToTableName = false)
            where T : class, new()
        {
            var entityType = typeof(T);
            this.CacheEntityClass(entityType);

            var tableName = this.TABLE_CACHE[entityType.FullName];
            return this.GetTableName(tableName, timestamp, tableExtenstionName, timestampAddToTableName);
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="originalTableName">原始表名</param>
        /// <param name="timestamp">数据时间</param>
        /// <param name="tableExtenstionName">表扩展名称,会追加到基本表名后: basci_tableExtenstionName</param>
        /// <param name="timestampAddToTableName">时间追加到表名</param>
        /// <returns>表名</returns>
        public string GetTableName(string originalTableName, DateTime? timestamp, string tableExtenstionName, bool timestampAddToTableName)
        {
            // 使用时间戳
            if (timestampAddToTableName && timestamp.HasValue)
            {
                // 表名 = 原始名称_表扩展名
                if (string.IsNullOrWhiteSpace(tableExtenstionName))
                {
                    return $"{originalTableName}{timestamp.Value.ToString("yyyyMM")}";
                }

                // 表名 = 原始名称_表扩展名yyyyMM
                return $"{originalTableName}_{tableExtenstionName}{timestamp.Value.ToString("yyyyMM")}";
            }

            // 表名 = 原始名称_表扩展名
            if (!string.IsNullOrWhiteSpace(tableExtenstionName))
            {
                return $"{originalTableName}_{tableExtenstionName}";
            }

            // 原始名称
            return originalTableName;
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
            // 表扩展名类型列计数
            var tableExtensionName = 0;
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
                    var ignoreAttr = property.GetCustomAttribute(typeof(IgnoreAttribute)) as IgnoreAttribute;
                    if (ignoreAttr != null)
                    {
                        info.ColumnType = Influx17xColumnType.Ignore;
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
                                if (timestamp == 1)
                                {
                                    throw new ArgumentException(
                                        $"存在多个 Timestamp 列"
                                        );
                                }

                                timestamp++;
                            }
                            break;
                        case Influx17xColumnType.TableExtensionName:
                            if (tableExtensionName == 1)
                            {
                                throw new ArgumentException(
                                       $"存在多个 TableExtensionName 列"
                                       );
                            }
                            tableExtensionName++;
                            break;
                    }
                    return info;

                })
                .Where(propertyInfo =>
                {
                    return propertyInfo.ColumnName != null
                    && propertyInfo.ColumnType != Influx17xColumnType.Ignore;
                })
                .ToArray();
        }

    }
}
