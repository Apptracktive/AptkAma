using System.Collections.Generic;

namespace Aptk.Plugins.AptkAma.Notification
{
    public interface IAptkAmaNotificationHandler
    {
        /// <summary>
        /// Google API Console App Project Numbers for Android notifications
        /// </summary>
        string[] GoogleSenderIds { get; }

        /// <summary>
        /// Called after any successful notification registration
        /// </summary>
        /// <param name="notificationName">Name of the notification</param>
        void OnRegistrationSucceeded(string notificationName);

        /// <summary>
        /// Called after any notification registration failure
        /// </summary>
        /// <param name="error">Error</param>
        void OnRegistrationFailed(string error);

        /// <summary>
        /// Called after any notification reception
        /// </summary>
        /// <param name="notification">The templated notification</param>
        void OnNotificationReceived(IAptkAmaNotification notification);

        /// <summary>
        /// Called after any successful notification unregistration
        /// </summary>
        /// <param name="notificationName">Name of the notification</param>
        void OnUnregistrationSucceeded(string notificationName);
    }
}
