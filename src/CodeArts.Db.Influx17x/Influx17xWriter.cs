using CodeArts.Db.Lts;

using System.Collections.Generic;

namespace CodeArts.Db
{
    public class Influx17xWriter : Writer
    {
        public Influx17xWriter(ISQLCorrectSettings settings, IWriterMap writer, Dictionary<string, object> parameters)
            : base(settings, writer, parameters)
        {
        }
        public override void Limit(string prefix)
        {

        }

        public override void NameWhiteSpace(string name, string alias)
        {
            Name(name);
        }
    }


}
