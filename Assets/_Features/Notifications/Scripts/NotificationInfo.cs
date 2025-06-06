using System;
using System.Collections.Generic;

namespace Quackery.Notifications
{
    public record NotificationInfo
    {
        public bool IsPersistent = false;
        public Notification Prefab;

        public Action<NotificationInfo> OnTapped;


    }
}
