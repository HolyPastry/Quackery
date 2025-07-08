using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Notifications
{
    public class NarrativeNotification : Notification
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Image _portrait;
        [SerializeField] private Image _banner;

        protected override void SetInfo(NotificationInfo info)
        {
            NarrativeNotificationInfo narrativeInfo = info as NarrativeNotificationInfo;
            _message.text = narrativeInfo.Narrative.Message;
            _banner.sprite = narrativeInfo.Narrative.Banner;
            _portrait.sprite = narrativeInfo.Narrative.Creator.Icon;
        }
    }
}
