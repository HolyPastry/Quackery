using System;

using UnityEngine;

namespace Quackery.Notifications
{
    [CreateAssetMenu(fileName = "NotificationInfo", menuName = "Quackery/Notifications/NotificationInfo")]
    public class NotificationData : ScriptableObject
    {
        public bool IsPersistent = false;
        public Notification Prefab;
        public NotificationExpandedPanel ExpandedPanelPrefab;
        public Action<NotificationData> OnTapped;
    }
}
