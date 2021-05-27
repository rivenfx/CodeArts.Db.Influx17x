
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
using CodeArts.Db.Lts;

namespace CodeArts.Db.Extenstions
{
    internal static class TypeExtenstions
    {
        public static Type FindGenericType(this Type type, Type definition)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
                {
                    return type;
                }

                if (definition.IsInterface)
                {
                    Type[] interfaces = type.GetInterfaces();

                    foreach (Type type2 in interfaces)
                    {
                        Type type3 = FindGenericType(type2, definition);

                        if (type3 is null) continue;

                        return type3;
                    }
                }

                type = type.BaseType;
            }
            return null;
        }
    }
}
