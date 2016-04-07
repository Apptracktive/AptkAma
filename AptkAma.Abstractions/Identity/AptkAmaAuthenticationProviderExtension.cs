using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Identity
{
    public static class AptkAmaAuthenticationProviderExtension
    {
        public static MobileServiceAuthenticationProvider ToMobileServiceAuthenticationProvider(
            this AptkAmaAuthenticationProvider authenticationProvider)
        {
            switch (authenticationProvider)
            {
                default:
                    return MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory;
                case AptkAmaAuthenticationProvider.MicrosoftAccount:
                    return MobileServiceAuthenticationProvider.MicrosoftAccount;
                case AptkAmaAuthenticationProvider.Google:
                    return MobileServiceAuthenticationProvider.Google;
                case AptkAmaAuthenticationProvider.Twitter:
                    return MobileServiceAuthenticationProvider.Twitter;
                case AptkAmaAuthenticationProvider.Facebook:
                    return MobileServiceAuthenticationProvider.Facebook;
            }
        }
    }
}
