namespace HQFramework.Resource
{
    public class ResourceLoadCompleteEventArgs : IReference
    {
        public uint crc
        {
            get;
            private set;
        }

        public object asset
        {
            get;
            private set;
        }

        public static ResourceLoadCompleteEventArgs Create(uint crc, object asset)
        {
            ResourceLoadCompleteEventArgs args = ReferencePool.Spawn<ResourceLoadCompleteEventArgs>();
            args.crc = crc;
            args.asset = asset;
            return args;
        }

        void IReference.OnRecyle()
        {
            crc = 0;
            asset = null;
        }
    }

    public class ResourceLoadCompleteEventArgs<T> : IReference where T : class
    {
        public uint crc
        {
            get;
            private set;
        }

        public T asset
        {
            get;
            private set;
        }

        public static ResourceLoadCompleteEventArgs<T> Create(uint crc, T asset)
        {
            ResourceLoadCompleteEventArgs<T> args = ReferencePool.Spawn<ResourceLoadCompleteEventArgs<T>>();
            args.crc = crc;
            args.asset = asset;
            return args;
        }

        void IReference.OnRecyle()
        {
            crc = 0;
            asset = null;
        }
    }
}
