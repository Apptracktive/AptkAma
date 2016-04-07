using Aptk.Plugins.AptkAma;
using Aptk.Plugins.AptkAma.Identity;
using AptkAma.Sample.Core.Helpers;
using Microsoft.WindowsAzure.MobileServices;

namespace AptkAma.Sample.Core.Services
{
    /// <summary>
    /// This IAptkAmaCacheService implementation is a working example 
    /// requiring the installation of Xamarin Settings plugin.
    /// </summary>
    public class AptkAmaCacheService : IAptkAmaCacheService
    {
        #region Identity
        public bool TryLoadCredentials(out IAptkAmaCredentials credentials)
        {
            credentials = !string.IsNullOrEmpty(Settings.AptkAmaIdentityUserId)
                          && !string.IsNullOrEmpty(Settings.AptkAmaIdentityAuthToken)
                          && Settings.AptkAmaIdentityProvider != AptkAmaAuthenticationProvider.None
                ? new AptkAmaCredentials(Settings.AptkAmaIdentityProvider, new MobileServiceUser(Settings.AptkAmaIdentityUserId)
                {
                    MobileServiceAuthenticationToken = Settings.AptkAmaIdentityAuthToken
                })
                : null;

            return credentials != null;
        }

        public void SaveCredentials(IAptkAmaCredentials credentials)
        {
            if (credentials == null)
                return;

            Settings.AptkAmaIdentityProvider = credentials.Provider;
            Settings.AptkAmaIdentityUserId = credentials.User.UserId;
            Settings.AptkAmaIdentityAuthToken = credentials.User.MobileServiceAuthenticationToken;
        }

        public void ClearCredentials()
        {
            Settings.AptkAmaIdentityProvider = AptkAmaAuthenticationProvider.None;
            Settings.AptkAmaIdentityUserId = string.Empty;
            Settings.AptkAmaIdentityAuthToken = string.Empty;
        }
        #endregion

        #region Notification
        public bool TryLoadRegistrationId(out string registrationId)
        {
            registrationId = Settings.AptkAmaNotificationRegistrationId;
            return !string.IsNullOrEmpty(registrationId);
        }

        public void SaveRegistrationId(string registrationId)
        {
            if (string.IsNullOrEmpty(registrationId))
                return;

            Settings.AptkAmaNotificationRegistrationId = registrationId;
        }

        public void ClearRegistrationId()
        {
            Settings.AptkAmaNotificationRegistrationId = string.Empty;
        }
        #endregion
    }
}
