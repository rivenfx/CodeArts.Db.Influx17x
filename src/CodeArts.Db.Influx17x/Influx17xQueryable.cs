
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using CodeArts.Db;
using CodeArts.Db.Extenstions;
using CodeArts.Db.Lts;

namespace CodeArts.Db
{
    public class Influx17xQueryable<T> : IRepository<T>, IQueryable<T>, IEnumerable<T>, IRepository, IQueryable, IQueryProvider, IEnumerable
    {
        // 当前连接配置
        protected readonly Influx17xConnectionConfig _connectionConfig;

        // 表达式
        protected readonly Expression _expression = null;

        protected readonly bool isEmpty = true;


        public Influx17xQueryable(Influx17xConnectionConfig connectionConfig, DateTime? start = null, DateTime? end = null)
        {
            if (connectionConfig is null)
            {
                throw new ArgumentNullException(nameof(connectionConfig));
            }
            this._connectionConfig = connectionConfig;

            this._expression = Expression.Constant(this);
            this.SelfDbContext = this.CreateDatabaseContext(
                connectionConfig,
                start,
                end
                );
        }

        protected Influx17xQueryable(Influx17xDbContext dbContext, Expression expression)
        {
            isEmpty = false;
            this.SelfDbContext = dbContext;
            this._expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }


        /// <summary>
        /// 表达式。
        /// </summary>
        public virtual Expression Expression => this._expression;

        /// <summary>
        /// 当前元素类型。
        /// </summary>
        public virtual Type ElementType => typeof(T);

        /// <summary>
        /// 查询供应器
        /// </summary>
        public virtual IQueryProvider Provider => this;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public virtual IDbContext DatabaseContext => this.SelfDbContext;

        /// <summary>
        /// 内部的数据库上下文
        /// </summary>
        protected virtual Influx17xDbContext SelfDbContext { get; }

        /// <summary>
        /// 迭代器。
        /// </summary>
        protected IEnumerable<T> Enumerable { private set; get; }


        #region IQueryable 实现

        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var type = expression.Type.FindGenericType(typeof(IQueryable<>));

            if (type is null)
            {
                throw new ArgumentException("无效表达式!", nameof(expression));
            }

            var type2 = typeof(Influx17xQueryable<>)
                .MakeGenericType(type.GetGenericArguments());

            var instance = Activator.CreateInstance(
                type2,
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic,
                null,
                new object[2] {
                    this.SelfDbContext,
                    expression
                },
                null);

            return (IQueryable)instance;
        }

        /// <summary>
        /// 创建IQueryable
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new Influx17xQueryable<TElement>(
                this.SelfDbContext,
                expression
            );
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return Execute<TResult>(expression);
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        object IQueryProvider.Execute(Expression expression)
        {
            return this.DatabaseContext.Read<T>(expression ?? throw new ArgumentNullException(nameof(expression)));
        }

        /// <summary>
        /// 执行结果表达式。
        /// </summary>
        /// <typeparam name="TResult">结果。</typeparam>
        /// <param name="expression">表达式。</param>
        /// <returns></returns>
        private TResult Execute<TResult>(Expression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!typeof(TResult).IsAssignableFrom(expression.Type))
            {
                throw new NotImplementedException(nameof(expression));
            }

            if (typeof(IEnumerable<T>).IsAssignableFrom(typeof(TResult)))
            {
                throw new NotSupportedException(nameof(expression));
            }

            return this.DatabaseContext.Read<TResult>(expression);
        }

        #endregion


        #region IEnumerable 实现

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        private IEnumerator<T> GetEnumerator()
        {
            if (isEmpty || Enumerable is null)
            {
                Enumerable = this.DatabaseContext.Query<T>(Expression);
            }

            return Enumerable.GetEnumerator();
        }


        #endregion


        #region 创建数据库适配器/查询器提供者

        /// <summary>
        /// 创建上下文。
        /// </summary>
        /// <param name="connectionConfig">链接配置。</param>
        /// <returns></returns>
        protected virtual Influx17xDbContext CreateDatabaseContext(Influx17xConnectionConfig connectionConfig, DateTime? start, DateTime? end)
        {
            return new Influx17xDbContext(connectionConfig, start, end);
        }


        #endregion


        #region From Table

        private static readonly Type TypeSelfEntity = typeof(T);

        private static readonly MethodInfo FromMethod = QueryableMethods.From.MakeGenericMethod(TypeSelfEntity);

        public virtual IQueryable<T> From(Func<ITableInfo, string> table)
        {
            var expression = Expression.Call(null, FromMethod, new Expression[2] { Expression, Expression.Constant(table ?? throw new ArgumentNullException(nameof(table))) });


            return new Influx17xQueryable<T>(
                this.SelfDbContext,
                expression
                );
        }

        #endregion


        #region ToString

        public override string ToString()
        {
            using (var visitor = this.SelfDbContext.QueryProvider.Create())
            {
                visitor.Startup(this.Expression);

                var sql = visitor.ToSQL();

                var parameters = visitor.Parameters;

                // 替换参数
                var parameterVal = string.Empty;
                foreach (var item in parameters)
                {

                    if (item.Value is DateTime dt)
                    {
                        parameterVal = dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
                    }
                    else
                    {
                        parameterVal = item.Value.ToString();
                    }

                    sql = sql.Replace($"@{item.Key}@", parameterVal);
                }

                return sql;
            }
        }

        #endregion
    }


}
