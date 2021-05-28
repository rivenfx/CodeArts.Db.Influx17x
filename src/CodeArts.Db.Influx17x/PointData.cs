using InfluxData.Net.InfluxDb.Models;

using System;
using System.Collections.Generic;

namespace CodeArts.Db
{
    public class PointData : Point
    {
        public PointData()
        {
            this.Tags = new Dictionary<string, object>();
            this.Fields = new Dictionary<string, object>();
        }

        public PointData(string measurement)
            : this()
        {
            this.Name = measurement.ToLower();
        }

        public virtual PointData Tag(string key, string value)
        {
            base.Tags[key] = value;
            return this;
        }

        public virtual PointData Field(string name, byte value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, float value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, double value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, decimal value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, long value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, ulong value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, uint value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData Field(string name, string value)
        {
            base.Fields[name.ToLower()] = value;
            return this;

        }

        public virtual PointData Field(string name, bool value)
        {
            base.Fields[name.ToLower()] = value;
            return this;
        }

        public virtual PointData TimeStamp(DateTime dateTime)
        {
            base.Timestamp = dateTime;
            return this;
        }

        public static PointData Measurement(string measurement)
        {
            return new PointData(measurement);
        }
    }
}
