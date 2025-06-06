using System;

namespace Quackery.Notifications
{
    public static class NotificationServices
    {
        public static Action ShowAllNotifications = delegate { };
        public static Action HideAllNotifications = delegate { };

        public static Action<NotificationInfo> ShowNotification = delegate { };
        public static Action<NotificationInfo> CloseNotification = delegate { };

        public static Action<NotificationInfo> ArchiveNotification = delegate { };
    }
}
