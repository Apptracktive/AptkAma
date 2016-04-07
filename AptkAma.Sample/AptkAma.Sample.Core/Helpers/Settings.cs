// Helpers/Settings.cs

using Aptk.Plugins.AptkAma.Identity;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace AptkAma.Sample.Core.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string AptkAmaIdentityUserIdKey = "AptkAmaIdentityUserId";
        private static readonly string AptkAmaIdentityUserIdDefault = string.Empty;
        private const string AptkAmaIdentityAuthTokenKey = "AptkAmaIdentityAuthToken";
        private static readonly string AptkAmaIdentityAuthTokenDefault = string.Empty;
        private const string AptkAmaIdentityProviderKey = "AptkAmaIdentityProvider";
        private static readonly AptkAmaAuthenticationProvider AptkAmaIdentityProviderDefault = AptkAmaAuthenticationProvider.None;
        private const string AptkAmaNotificationRegistrationIdKey = "AptkAmaNotificationRegistrationId";
        private static readonly string AptkAmaNotificationRegistrationIdDefault = string.Empty;

        #endregion


        public static string AptkAmaIdentityUserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(AptkAmaIdentityUserIdKey, AptkAmaIdentityUserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AptkAmaIdentityUserIdKey, value);
            }
        }

        public static string AptkAmaIdentityAuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(AptkAmaIdentityAuthTokenKey, AptkAmaIdentityAuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AptkAmaIdentityAuthTokenKey, value);
            }
        }

        public static AptkAmaAuthenticationProvider AptkAmaIdentityProvider
        {
            get
            {
                return AppSettings.GetValueOrDefault(AptkAmaIdentityProviderKey, AptkAmaIdentityProviderDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AptkAmaIdentityProviderKey, value);
            }
        }

        public static string AptkAmaNotificationRegistrationId
        {
            get
            {
                return AppSettings.GetValueOrDefault(AptkAmaNotificationRegistrationIdKey, AptkAmaNotificationRegistrationIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AptkAmaNotificationRegistrationIdKey, value);
            }
        }
    }
}