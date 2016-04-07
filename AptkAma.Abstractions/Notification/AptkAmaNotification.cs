using System.Collections.Generic;
using System.Linq;

namespace Aptk.Plugins.AptkAma.Notification
{
    public class AptkAmaNotification : Dictionary<string, object>, IAptkAmaNotification
    {
        public bool IsTypeOf<T>(T notificationTemplate) where T : class, IAptkAmaNotificationTemplate
        {
            if (notificationTemplate.ContainsKey("name"))
            {
                object name;
                return this.TryGetValue("name", out name) && notificationTemplate.Name == name.ToString();
            }

            return this.Count == notificationTemplate.Count && this.Keys.SequenceEqual(notificationTemplate.Keys);
        }
    }
}
