
using System.Collections;
using UnityEngine;

namespace Quackery.Notifications
{
    public class NotificationTest : MonoBehaviour
    {
        [SerializeField] private NotificationApp _notificationApp;

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();

            _notificationApp.Show();
        }

        private void NotificationOnTapped(NotificationInfo info)
        {
            Debug.Log($"Notification tapped: {info}");
        }
    }
}
