using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Identity
{
    public class AptkAmaCredentials : IAptkAmaCredentials
    {
        public AptkAmaCredentials(AptkAmaAuthenticationProvider provider, MobileServiceUser user)
        {
            Provider = provider;
            User = user;
        }

        /// <summary>
        /// Identity provider for this User
        /// </summary>
        public AptkAmaAuthenticationProvider Provider { get; }

        /// <summary>
        /// Mobile service user
        /// </summary>
        public MobileServiceUser User { get; }
    }
}
