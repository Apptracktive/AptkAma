namespace Aptk.Plugins.AptkAma.Identity
{
    /// <summary>
    /// Authentication providers
    /// </summary>
    public enum AptkAmaAuthenticationProvider
    {
        /// <summary>
        /// Default null authentication provider.
        /// </summary>
        None = 0,
        /// <summary>
        /// Microsoft Account authentication provider.
        /// </summary>
        MicrosoftAccount = 1,
        /// <summary>
        /// Google authentication provider.
        /// </summary>
        Google = 2,
        /// <summary>
        /// Twitter authentication provider.
        /// </summary>
        Twitter = 3,
        /// <summary>
        /// Facebook authentication provider.
        /// </summary>
        Facebook = 4,
        /// <summary>
        /// Azure Active Directory authentication provider.
        /// </summary>
        WindowsAzureActiveDirectory = 5,
        /// <summary>
        /// Custom authentication provider.
        /// </summary>
        Custom = 6
    }
}