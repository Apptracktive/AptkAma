using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aptk.Plugins.AptkAma.Notification
{
    public interface IAptkAmaNotification : IDictionary<string, object>
    {
        bool IsTypeOf<T>(T notificationTemplate) where T : class, IAptkAmaNotificationTemplate;
    }
}
