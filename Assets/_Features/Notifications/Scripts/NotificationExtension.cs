using UnityEngine;

namespace Quackery.Notifications
{
    public abstract class NotificationExtension : MonoBehaviour
    {
        public abstract NotificationInfo Info { get; }
    }
}
