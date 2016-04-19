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
        /// Notification name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Registration subscription tags
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
