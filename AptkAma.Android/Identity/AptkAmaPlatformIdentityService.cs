using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.CurrentActivity;

namespace Aptk.Plugins.AptkAma.Identity
{
    public class AptkAmaPlatformIdentityService : IAptkAmaPlatformIdentityService
    {
        private readonly IMobileServiceClient _client;

        public AptkAmaPlatformIdentityService(IMobileServiceClient client)
        {
            _client = client;
        }

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null, bool useSingleSignOnIfAvailable = false)
        {
            return await _client.LoginAsync(CrossCurrentActivity.Current.Activity, provider, parameters);
        }
    }
}