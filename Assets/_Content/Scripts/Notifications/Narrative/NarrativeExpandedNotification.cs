using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Notifications
{
    public class NarrativeExpandedNotification : NotificationExpandedPanel
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Image _portrait;
        [SerializeField] private Image _banner;

        public override IEnumerator Init(NotificationInfo info)
        {
            _inProgress = true;
            _animatedRect.ZoomIn();
            NarrativeNotificationInfo narrativeInfo = info as NarrativeNotificationInfo;

            _banner.sprite = narrativeInfo.Narrative.Banner;
            _portrait.sprite = narrativeInfo.Narrative.Creator.Icon;
            _message.text = narrativeInfo.Narrative.Message;
            yield return null;
        }
    }
}
