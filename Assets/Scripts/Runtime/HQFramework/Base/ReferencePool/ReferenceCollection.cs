using System;
using System.Collections.Generic;

namespace HQFramework
{
    internal class ReferenceCollection
    {
        private readonly Stack<IReference> references;
        private readonly Type refType;
        private ushort totalCount;
        private ushort usingCount;
        private ushort unusedCount;
        private readonly ushort maxCapacity;

        public ushort TotalCount => totalCount;
        public ushort UsingCount => usingCount;
        public ushort UnusedCount => unusedCount;

        public ReferenceCollection(Type refType, ushort capacity, ushort maxCapacity)
        {
            references = new Stack<IReference>(capacity);
            this.refType = refType;
            this.maxCapacity = maxCapacity;
        }

        public T Spawn<T>() where T : class, IReference, new()
        {
            if (typeof(T) != refType)
            {
                throw new Exception("Type is invalid");
            }
            return Spawn() as T;
        }

        public IReference Spawn()
        {
            if (unusedCount == 0)
            {
                IReference reference = Activator.CreateInstance(refType) as IReference;
                if (totalCount < maxCapacity)
                {
                    totalCount++;
                    usingCount++;
                }
                return reference;
            }
            else
            {
                lock (references)
                {
                    IReference reference = references.Pop();
                    usingCount++;
                    unusedCount--;
                    return reference;
                }
            }
        }

        public void Recyle(IReference reference)
        {
            if (totalCount >= maxCapacity)
            {
                //($"{refType} pool has run out of max capacity, this will not be recycled");
                return;
            }

            unusedCount++;
            usingCount--;
            lock (references)
            {
                reference.OnRecyle();
                references.Push(reference);
            }
        }

        public void Shrink(ushort count)
        {
            lock (references)
            {
                if (count > totalCount)
                {
                    count = totalCount;
                }
                totalCount -= count;
                unusedCount -= count;
                while (count-- > 0)
                {
                    references.Pop();
                }
            }
        }

        public void Clear()
        {
            lock (references)
            {
                references.Clear();
                totalCount = usingCount = unusedCount = 0;
            }
        }
    }
}
