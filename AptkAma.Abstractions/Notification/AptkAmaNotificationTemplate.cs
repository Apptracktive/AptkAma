using System;
using System.Collections.Generic;
using System.Globalization;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Notification to subscribe for
    /// </summary>
    public class AptkAmaNotificationTemplate : Dictionary<string, object>, IAptkAmaNotificationTemplate
    {
        /// <summary>
        /// Create a new Notification registration with its template.
        /// Tags and ExpiresAfter properties at default means a registration for all notifications, expriring after 90 days.
        /// </summary>
        /// <param name="name">Notification name</param>
        public AptkAmaNotificationTemplate(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Create a new Notification registration with its template.
        /// ExpiresAfter property at default means notifications expriring after 90 days.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tags">Tags to subscribe for</param>
        public AptkAmaNotificationTemplate(string name, IEnumerable<string> tags)
        {
            Name = name;
            Tags = tags;
        }

        /// <summary>
        /// Create a new Notification registration with its template.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tags">Tags to subscribe for</param>
        /// <param name="expiresAfter">Notification expiration delay</param>
        /// <param name="cultureInfo">Notification expiration date culture info</param>
        public AptkAmaNotificationTemplate(string name, IEnumerable<string> tags, TimeSpan expiresAfter, CultureInfo cultureInfo)
        {
            Name = name;
            Tags = tags;
            ExpiresAfter = expiresAfter;
            CultureInfo = cultureInfo;
        }

        /// <summary>
        /// Notification name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Registration subscription tags
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Notification expiration delay (used with iOS only)
        /// </summary>
        /// <value>90 days</value>
        public TimeSpan ExpiresAfter { get; set; } = TimeSpan.FromDays(90);

        /// <summary>
        /// Expiration date culture info (used with iOS only)
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = new CultureInfo("en-US");
    }
}
