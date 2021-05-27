using CodeArts.Casting;
using CodeArts.Db.Lts;

using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArts.Db
{
    public static class CodeArtsHelper
    {
        public static bool Initialized { get; private set; }
        public static void InitializeBasic()
        {
            if (Initialized)
            {
                return;
            }
            DbConnectionManager.RegisterProvider<CodeArtsProvider>();
            RuntimeServPools.TryAddSingleton<IMapper, CastingMapper>();
        }
    }
}
