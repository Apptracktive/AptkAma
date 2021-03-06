﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Aptk.Plugins.AptkAma.Notification
{
    /// <summary>
    /// Notification to subscribe for
    /// </summary>
    public interface IAptkAmaNotificationTemplate : IDictionary<string, object>
    {
        /// <summary>
        /// Notification name
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Registration subscription tags
        /// </summary>
        IEnumerable<string> Tags { get; set; }
    }
}