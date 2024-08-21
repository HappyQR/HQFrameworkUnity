using System;
using System.Collections.Generic;

namespace HQFramework
{
    public sealed class ReferencePool
    {
        private static Dictionary<Type, ReferenceCollection> poolDic = new Dictionary<Type, ReferenceCollection>();

        public static int Count => poolDic.Count;

        public static void RegisterPool(Type refType, ushort capacity = 8, ushort maxCapacity = 128)
        {
            lock (poolDic)
            {
                if (poolDic.ContainsKey(refType))
                {
                    throw new ArgumentException($"You have registered {refType} Pool, cannot do it again.");
                }
                poolDic.Add(refType, new ReferenceCollection(refType, capacity, maxCapacity));
            }
        }

        public static IReference Spawn(Type refType)
        {
            return GetReferenceCollection(refType).Spawn();
        }

        public static T Spawn<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Spawn<T>();
        }

        public static void Recyle(IReference target)
        {
            Type refType = target.GetType();
            GetReferenceCollection(refType).Recyle(target);
        }

        public static void Shrink(Type refType, ushort count)
        {
            GetReferenceCollection(refType).Shrink(count);
        }

        public static void Shrink<T>(ushort count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Shrink(count);
        }

        public static void Delete(Type refType)
        {
            GetReferenceCollection(refType).Clear();
            lock (poolDic)
            {
                poolDic.Remove(refType);
            }
        }

        public static void Delete<T>() where T : class, IReference, new()
        {
            Type refType = typeof(T);
            GetReferenceCollection(refType).Clear();
            lock (poolDic)
            {
                poolDic.Remove(refType);
            }
        }

        public static void ClearAll()
        {
            lock (poolDic)
            {
                foreach (var item in poolDic)
                {
                    item.Value.Clear();
                }
                poolDic.Clear();
            }
        }

        private static ReferenceCollection GetReferenceCollection(Type refType)
        {
            ReferenceCollection referenceCollection = null;
            lock (poolDic)
            {
                if (!poolDic.ContainsKey(refType))
                {
                    RegisterPool(refType);
                }
                referenceCollection = poolDic[refType];
            }

            return referenceCollection;
        }
    }
}
