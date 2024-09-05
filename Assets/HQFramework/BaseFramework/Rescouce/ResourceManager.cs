namespace HQFramework.Resource
{
    internal sealed partial class ResourceManager : HQModuleBase, IResourceManager
    {
        private IResourceHelper resourceHelper;
        private ResourceConfig config;
        public override byte Priority => byte.MaxValue;

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
