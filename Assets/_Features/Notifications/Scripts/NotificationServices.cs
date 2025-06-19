using System;

namespace Quackery.Notifications
{
    public static class NotificationServices
    {

        public static Action RemoveAllNotifications = delegate { };

        public static Action<NotificationInfo> ShowNotification = delegate { };
        public static Action<NotificationInfo> CloseNotification = delegate { };

        public static Action<NotificationInfo> ArchiveNotification = delegate { };

        internal static Action<NotificationInfo, float> ShowNotificationWithDelay = delegate { };
        internal static Action<NotificationInfo> ShowExpandedPanel = delegate { };

        internal static Action GenerateDailyNotification = delegate { };


    }
}
