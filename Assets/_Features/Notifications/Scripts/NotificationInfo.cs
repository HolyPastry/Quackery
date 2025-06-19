using System;
using System.Collections.Generic;
using UnityEditor;

namespace Quackery.Notifications
{
    [Serializable]
    public record NotificationInfo
    {
        public bool IsPersistent = false;
        public Notification Prefab;
        public NotificationExpandedPanel ExpandedPanelPrefab;
        public Action<NotificationInfo> OnTapped;
    }
}
