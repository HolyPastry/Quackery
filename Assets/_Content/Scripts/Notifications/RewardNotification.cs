
using System.Collections.Generic;

using Quackery.Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Quackery
{

    public record RewardNotificationInfo : NotificationInfo
    {
        public Sprite Portrait;
        public int Rating;
        public string ReviewText;
    }

    public class RewardNotification : Notification
    {
        [SerializeField] private Image _portraitImage;
        [SerializeField] private TextMeshProUGUI _reviewText;
        [SerializeField] private List<GameObject> _ratingStars;
        protected override void SetInfo(NotificationInfo info)
        {
            if (info is not RewardNotificationInfo rewardInfo) return;
            _portraitImage.sprite = rewardInfo.Portrait;
            _reviewText.text = rewardInfo.ReviewText;
            for (int i = 0; i < _ratingStars.Count; i++)
            {
                _ratingStars[i].SetActive(i < rewardInfo.Rating);
            }
        }
    }


}
