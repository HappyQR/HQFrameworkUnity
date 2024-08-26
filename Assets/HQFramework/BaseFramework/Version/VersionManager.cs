using System;
using System.Net.Http;

namespace HQFramework.Version
{
    internal class VersionManager : HQModuleBase, IVersionManager
    {
        private IVersionHelper versionHelper;

        public override byte Priority => byte.MaxValue;
        public event Action<VersionCheckResult> onVersionCheckCallback;
        public event Action<string> onVersionCheckError;

        public void SetVersionHelper(IVersionHelper helper)
        {
            this.versionHelper = helper;
        }

        public void CheckVersion()
        {
            CheckVersionInternal();
        }

        public VersionInfo GetLocalVersionInfo()
        {
            return versionHelper.GetLocalVersionInfo();
        }

        private async void CheckVersionInternal()
        {
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            try
            {
                string versionJsonStr = await client.GetStringAsync(versionHelper.RemoteVersionUrl);
                VersionInfo remoteVersionInfo = SerializeManager.JsonToObject<VersionInfo>(versionJsonStr);
                VersionInfo localVersionInfo = versionHelper.GetLocalVersionInfo();

                VersionCheckResult result = CompareVersion(localVersionInfo, remoteVersionInfo);
                if (result == null)
                {
                    onVersionCheckError?.Invoke("Not a compatible version, please check the project's config.");
                }
                else
                {
                    onVersionCheckCallback?.Invoke(result);
                }
            }
            catch (Exception ex)
            {
                onVersionCheckError?.Invoke(ex.Message);
            }
        }

        private VersionCheckResult CompareVersion(VersionInfo localVersion, VersionInfo remoteVersion)
        {
            if (localVersion.productName != remoteVersion.productName ||
                localVersion.companyName != remoteVersion.companyName ||
                localVersion.runtimePlatform != remoteVersion.runtimePlatform)
            {
                return null;
            }
            else
            {
                if (remoteVersion.internalVersionCode == localVersion.internalVersionCode)
                {
                    VersionCheckResult result = new VersionCheckResult(VersionCheckResult.UpdateType.Latest, null);
                    return result;
                }
                else
                {
                    VersionCheckResult.UpdateType updateType = remoteVersion.minimalSupportedVersionCode <= localVersion.internalVersionCode? VersionCheckResult.UpdateType.SuggestedUpdate : VersionCheckResult.UpdateType.ForceUpdate;
                    VersionCheckResult result = new VersionCheckResult(updateType, remoteVersion);
                    return result;
                }
            }
        }
    }
}
