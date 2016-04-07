using Aptk.Plugins.AptkAma.Identity;

namespace Aptk.Plugins.AptkAma
{
    public interface IAptkAmaCacheService
    {
        #region Identity
        /// <summary>
        /// Try to load previously saved credentials
        /// </summary>
        /// <param name="credentials">Saved credentials</param>
        /// <returns>Load success</returns>
        bool TryLoadCredentials(out IAptkAmaCredentials credentials);

        /// <summary>
        /// Save actual credentials
        /// </summary>
        /// <param name="credentials">Credentials to save</param>
        void SaveCredentials(IAptkAmaCredentials credentials);

        /// <summary>
        /// Clear saved credentials
        /// </summary>
        void ClearCredentials();
        #endregion
        
        #region Notification
        /// <summary>
        /// Try to load previously saved notification registration Id
        /// </summary>
        /// <param name="registrationId">Saved registration Id</param>
        /// <returns>Load success</returns>
        bool TryLoadRegistrationId(out string registrationId);

        /// <summary>
        /// Save actual notification registration id
        /// </summary>
        /// <param name="registrationId">Notification registration Id to save</param>
        void SaveRegistrationId(string registrationId);

        /// <summary>
        /// Clear saved notification registration id
        /// </summary>
        void ClearRegistrationId();
        #endregion
    }
}
