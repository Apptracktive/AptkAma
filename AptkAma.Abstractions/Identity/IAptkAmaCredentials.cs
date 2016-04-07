using Microsoft.WindowsAzure.MobileServices;

namespace Aptk.Plugins.AptkAma.Identity
{
    public interface IAptkAmaCredentials
    {
        /// <summary>
        /// Identity provider for this User
        /// </summary>
        AptkAmaAuthenticationProvider Provider { get; }

        /// <summary>
        /// Mobile service user
        /// </summary>
        MobileServiceUser User { get; }
    }
}
