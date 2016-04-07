using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aptk.Plugins.AptkAma.Notification;

namespace Aptk.Plugins.AptkAma.Extensions
{
    public static class DictionaryExtensions
    {
        public static IAptkAmaNotification ToNotification(this IDictionary<string, object> source)
        {
            var notification = new AptkAmaNotification();
            foreach (var item in source)
            {
                notification.Add(item.Key, item.Value);
            }

            return notification;
        }
    }
}
