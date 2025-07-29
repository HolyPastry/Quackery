using System;
using UnityEngine;

namespace Quackery.Notifications
{
    public static class NotificationServices
    {

        public static Action RemoveWeeklyNotifications = delegate { };

        public static Action<NotificationInfo> ShowNotification = delegate { };

        public static Action<NotificationInfo> ArchiveNotification = delegate { };
        internal static Action<NotificationInfo> ShowExpandedPanel = delegate { };

        internal static Action GenerateWeeklyNotification = delegate { };

        internal static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);

    }
}
