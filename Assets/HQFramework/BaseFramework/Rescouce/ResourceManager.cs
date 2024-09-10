namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private ResourceConfig config;
        public override byte Priority => byte.MaxValue;

        public static readonly string manifestFileName = "AssetModuleManifest.json";

        protected override void OnInitialize()
        {
            
        }

        public void SetHelper(IResourceHelper resourceHelper)
        {
            this.resourceHelper = resourceHelper;
            config = resourceHelper.LoadResourceConfig();
        }

        public void DecompressBuiltinResource()
        {
            
        }


    }
}
