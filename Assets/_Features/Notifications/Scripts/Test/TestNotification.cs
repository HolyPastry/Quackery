
using TMPro;
using UnityEngine;

namespace Quackery.Notifications
{
    public class TestNotificationInfo : NotificationData
    {
        public string Message;
    }

    public class TestNotification : Notification
    {
        [SerializeField] private TextMeshProUGUI _message;
        protected override void SetInfo(NotificationInfo info)
        {
            var testInfo = info.Data as TestNotificationInfo;
            _message.text = testInfo.Message;
        }
    }
}
