using System;
using System.Collections.Generic;

namespace HQFramework
{
    public static partial class Utility
    {
        public static class Assembly
        {
            private static readonly System.Reflection.Assembly[] assemblies;
            private static readonly Dictionary<string, Type> cachedTypeDic;

            static Assembly()
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
                cachedTypeDic = new Dictionary<string, Type>();
            }

            public static Type GetType(string fullName)
            {
                if(cachedTypeDic.ContainsKey(fullName))
                {
                    return cachedTypeDic[fullName];
                }
                else
                {
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        Type type = Type.GetType($"{fullName}, {assemblies[i].FullName}");
                        if (type != null)
                        {
                            cachedTypeDic.Add(fullName, type);
                            return type;
                        }
                    }
                }
                return null;
            }
        }
    }
}
