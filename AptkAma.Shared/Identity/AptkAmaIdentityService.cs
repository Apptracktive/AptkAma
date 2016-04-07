using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Identity
{
    public class AptkAmaIdentityService : AptkAmaBaseIdentityService
    {
        private readonly IAptkAmaPluginConfiguration _configuration;

        public AptkAmaIdentityService(IAptkAmaPluginConfiguration configuration, IMobileServiceClient client) : base(configuration, client)
        {
            _configuration = configuration;
        }

        public override async Task<MobileServiceUser> LoginAsync(AptkAmaAuthenticationProvider provider, IDictionary<string, string> parameters = null, bool useSingleSignOnIfAvailable = false)
        {
            if (provider == AptkAmaAuthenticationProvider.None || provider == AptkAmaAuthenticationProvider.Custom)
                throw new ArgumentException("AptkAmaAuthenticationProvider must be of type MicrosoftAccount, Google, Twitter, Facebook or WindowsAzureActiveDirectory");

            var user = await AptkAmaPluginLoader.PlatformIdentityInstance.LoginAsync(provider.ToMobileServiceAuthenticationProvider(), parameters, useSingleSignOnIfAvailable);

            _configuration.CacheService?.SaveCredentials(new AptkAmaCredentials(provider, user));

            return user;
        }
    }
}
