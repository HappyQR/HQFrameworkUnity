namespace HQFramework.Resource
{
    public class ResourceLoadErrorEventArgs : IReference
    {
        public uint crc
        {
            get;
            private set;
        }

        public string assetPath
        {
            get;
            private set;
        }

        public string errorMessage
        {
            get;
            private set;
        }

        public static ResourceLoadErrorEventArgs Create(uint crc, string assetPath, string errorMessage)
        {
            ResourceLoadErrorEventArgs args = ReferencePool.Spawn<ResourceLoadErrorEventArgs>();
            args.crc = crc;
            args.assetPath = assetPath;
            args.errorMessage = errorMessage;
            return args;
        }

        void IReference.OnRecyle()
        {
            crc = 0;
            assetPath = null;
            errorMessage = null;
        }
    }
}
