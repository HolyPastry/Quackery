
using System.Collections;
using UnityEngine;

namespace Quackery.Notifications
{
    public class NotificationTest : MonoBehaviour
    {
        [SerializeField] private TestNotification _notificationPrefab;
        [SerializeField] private string _message;
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();

            var info = new TestNotificationInfo
            {
                IsPersistent = true,
                Message = _message,
                Prefab = _notificationPrefab,
                OnTapped = NotificationOnTapped
            };

            NotificationServices.ShowNotification?.Invoke(info);
            NotificationServices.ShowNotification?.Invoke(info);

            yield return new WaitForSeconds(1f);
            NotificationServices.HideAllNotifications?.Invoke();
            yield return new WaitForSeconds(1f);
            NotificationServices.ShowAllNotifications?.Invoke();

            NotificationServices.CloseNotification?.Invoke(info);
            yield return new WaitForSeconds(1f);
            NotificationServices.ArchiveNotification?.Invoke(info);

            var info2 = new TestNotificationInfo
            {
                IsPersistent = false,
                Message = "This is a transient notification",
                Prefab = _notificationPrefab,
                OnTapped = NotificationOnTapped
            };
            NotificationServices.ShowNotification?.Invoke(info2);
            yield return new WaitForSeconds(6f);

            NotificationServices.ShowNotification?.Invoke(info);
        }

        private void NotificationOnTapped(NotificationInfo info)
        {
            Debug.Log($"Notification tapped: {info}");
        }
    }
}
