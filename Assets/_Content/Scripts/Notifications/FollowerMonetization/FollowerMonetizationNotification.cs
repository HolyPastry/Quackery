using TMPro;
using UnityEngine;

namespace Quackery.Notifications
{
    public class FollowerMonetizationNotification : Notification
    {
        [SerializeField] private TextMeshProUGUI _message;
        protected override void SetInfo(NotificationInfo info)
        {
            var monetizationInfo = info as FollowerMonetizationNotificationInfo;
            _message.text = $"You earned {monetizationInfo.EarnedCash} from followers!";
        }
    }
}
