namespace HQFramework.Resource
{
    public class ManifestLoadCompleteEventArgs
    {
        public readonly HQAssetManifest manifest;

        public ManifestLoadCompleteEventArgs(HQAssetManifest manifest)
        {
            this.manifest = manifest;
        }
    }
}
