using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;

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
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var root = window.RootViewController;

            if(root == null)
                throw new NullReferenceException("RootViewController should not be null when trying to log in");

            var current = root;
            while (current.PresentedViewController != null)
            {
                current = current.PresentedViewController;
            }
            return await _client.LoginAsync(current, provider, parameters);
        }
    }
}
