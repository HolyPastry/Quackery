
using System.Collections;
using UnityEngine;

namespace Quackery.Notifications
{
    public class NotificationTest : MonoBehaviour
    {
        [SerializeField] private NotificationApp _notificationApp;

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();

            _notificationApp.Show();
        }

        private void NotificationOnTapped(NotificationData info)
        {
            Debug.Log($"Notification tapped: {info}");
        }
    }
}
