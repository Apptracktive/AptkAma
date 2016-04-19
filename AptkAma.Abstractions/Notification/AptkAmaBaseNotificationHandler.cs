namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Base notification handling class
    /// </summary>
    public abstract class AptkAmaBaseNotificationHandler : IAptkAmaNotificationHandler
    {
        /// <summary>
        /// Google API Console App Project Numbers for Android notifications
        /// </summary>
        public string[] GoogleSenderIds { get; protected set; }

        /// <summary>
        /// Called after any successful notification registration
        /// </summary>
        public virtual void OnRegistrationSucceeded()
        {
        }

        /// <summary>
        /// Called after any notification registration failure
        /// </summary>
        /// <param name="error">Error</param>
        public virtual void OnRegistrationFailed(string error)
        {
        }

        /// <summary>
        /// Called after any notification reception
        /// </summary>
        /// <param name="notification">The templated notification</param>
        public abstract void OnNotificationReceived(IAptkAmaNotification notification);

        /// <summary>
        /// Called after any successful notification unregistration
        /// </summary>
        public virtual void OnUnregistrationSucceeded()
        {
        }
    }
}
