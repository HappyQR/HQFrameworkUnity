using System.Net.Http;

namespace HQFramework.WebRequest
{
    internal class WebRequestManager : HQModuleBase, IWebRequestManager
    {
        private HttpClient client;

        public override byte Priority => byte.MaxValue;

        protected override void OnInitialize()
        {
            client = new HttpClient();
        }

        protected override void OnShutdown()
        {
            client.CancelPendingRequests();
            client.Dispose();
        }
    }
}
