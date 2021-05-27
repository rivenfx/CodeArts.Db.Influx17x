using CodeArts.Db.Lts;
using CodeArts.Db.Lts.Visitors;

using System.Collections.Generic;

namespace CodeArts.Db
{
    public class Influx17xQueryVisitor : QueryVisitor
    {
        public Influx17xQueryVisitor(ISQLCorrectSettings settings, ICustomVisitorList visitors) : base(settings, visitors)
        {
        }

        protected override Writer CreateWriter(ISQLCorrectSettings settings, IWriterMap writeMap, Dictionary<string, object> parameters)
        {
            return new Influx17xWriter(settings, writeMap, parameters);
        }
    }


}
