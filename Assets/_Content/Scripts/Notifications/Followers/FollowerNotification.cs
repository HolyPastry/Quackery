using Quackery.Followers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Notifications
{

    public class FollowerNotification : Notification
    {
        [SerializeField] private TextMeshProUGUI _followerCount;
        [SerializeField] private Image _levelIcon;
        [SerializeField] private TextMeshProUGUI _nextLevelTease;

        protected override void SetInfo(NotificationInfo _)
        {

            int followerCount = FollowerServices.GetNumberOfFollowers();
            var currentLevel = FollowerServices.GetCurrentLevel();
            var numToNextLevel = FollowerServices.GetNumFollowersToNextLevel();

            _followerCount.text = $"{followerCount.ToString("0")} followers!";

            _levelIcon.sprite = currentLevel.Icon;
            if (numToNextLevel < 0)
                _nextLevelTease.text = "Max Level Reached!";
            else
                _nextLevelTease.text = $"only {numToNextLevel.ToString("0")} more to the next level!";



        }
    }
}
