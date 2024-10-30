namespace HQFramework
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

        public string bundleName
        {
            get;
            private set;
        }

        public string moduleName
        {
            get;
            private set;
        }

        public string errorMessage
        {
            get;
            private set;
        }

        public static ResourceLoadErrorEventArgs Create(uint crc, string assetPath, string bundleName, string moduleName, string errorMessage)
        {
            ResourceLoadErrorEventArgs args = ReferencePool.Spawn<ResourceLoadErrorEventArgs>();
            args.crc = crc;
            args.assetPath = assetPath;
            args.bundleName = bundleName;
            args.moduleName = moduleName;
            args.errorMessage = errorMessage;
            return args;
        }

        void IReference.OnRecyle()
        {
            crc = 0;
            assetPath = null;
            bundleName = null;
            moduleName = null;
            errorMessage = null;
        }
    }
}
