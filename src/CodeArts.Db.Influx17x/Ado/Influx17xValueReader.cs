using InfluxData.Net.InfluxDb.Models.Responses;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Linq;

namespace CodeArts.Db.Ado
{
    public class Influx17xDataReader : DbDataReader
    {
        bool _hasRows;

        protected IList<string> _columns;
        protected IList<IList<object>> _records;

        protected Dictionary<string, int> _columnIndexDict;
        protected IEnumerator<IList<object>> _recordsEnumerator;


        public Influx17xDataReader()
        {

        }

        public Influx17xDataReader(IList<string> columns, IList<IList<object>> dataSouce)
        {
            this.SetOptions(columns, dataSouce);
        }



        public override object this[int ordinal] => this.GetValue(ordinal);

        public override object this[string name] => this.GetValue(
            this.GetOrdinal(name)
            );

        public override int Depth => 0;

        public override int FieldCount => _columns?.Count ?? 0;

        public override bool HasRows => _hasRows;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override bool GetBoolean(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is bool res)
            {
                return res;
            }
            else
            {
                return bool.Parse(obj.ToString());
            }
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            var text = this.GetString(ordinal);
            if (text?.Length == 1)
            {
                return text[0];
            }

            return checked((char)GetInt64(ordinal));
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            var text = this.GetString(ordinal);

            int charsToRead = text.Length - (int)dataOffset;
            charsToRead = Math.Min(charsToRead, length);
            text.CopyTo((int)dataOffset, buffer, bufferOffset, charsToRead);
            return charsToRead;
        }

        public override string GetDataTypeName(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            return obj?.GetType()?.FullName;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is DateTime res)
            {
                return res;
            }
            else
            {
                return DateTime.Parse(obj.ToString());
            }
        }

        public override decimal GetDecimal(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is decimal res)
            {
                return res;
            }
            else
            {
                return decimal.Parse(obj.ToString());
            }
        }

        public override double GetDouble(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is double res)
            {
                return res;
            }
            else
            {
                return double.Parse(obj.ToString());
            }
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, false);
        }

        public override Type GetFieldType(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            return obj?.GetType() ?? null;
        }

        public override float GetFloat(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is float res)
            {
                return res;
            }
            else
            {
                return float.Parse(obj.ToString());
            }
        }

        public override Guid GetGuid(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is Guid res)
            {
                return res;
            }
            else
            {
                return Guid.Parse(obj.ToString());
            }
        }

        public override short GetInt16(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is short res)
            {
                return res;
            }
            else
            {
                return short.Parse(obj.ToString());
            }
        }

        public override int GetInt32(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is int res)
            {
                return res;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public override long GetInt64(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is long res)
            {
                return res;
            }
            else
            {
                return long.Parse(obj.ToString());
            }
        }

        public override string GetName(int ordinal)
        {
            return this._columns[ordinal];
        }

        public override int GetOrdinal(string name)
        {
            if (this._columnIndexDict.TryGetValue(name, out int res))
            {
                return res;
            }

            throw new ArgumentOutOfRangeException(nameof(name), name, message: null);
        }

        public override string GetString(int ordinal)
        {
            var obj = this.GetValue(ordinal);
            if (obj is string res)
            {
                return res;
            }

            return obj?.ToString();
        }

        public override object GetValue(int ordinal)
        {
            return this._recordsEnumerator.Current[ordinal];
        }

        public override int GetValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = this._recordsEnumerator.Current[i];
            }

            return values.Length;
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.GetValue(ordinal) == null;
        }

        public override bool NextResult()
        {
            return this._recordsEnumerator?.MoveNext() ?? false;
        }

        public override bool Read()
        {
            return this.NextResult();
        }


        public virtual void SetOptions(IList<string> columns, IList<IList<object>> dataSouce)
        {
            this._recordsEnumerator?.Dispose();

            this._columns = columns;
            this._records = dataSouce;

            this._hasRows = dataSouce.Any(a => a.Count > 0);
            this._recordsEnumerator = dataSouce.GetEnumerator();

            this._columnIndexDict = new Dictionary<string, int>();
            for (int i = 0; i < this._columns.Count; i++)
            {
                this._columnIndexDict[_columns[i]] = i;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this._recordsEnumerator?.Dispose();
        }
    }
}
