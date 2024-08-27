namespace HQFramework.Resource
{
    internal class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private ResourceConfig config;
        public override byte Priority => 10;

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
