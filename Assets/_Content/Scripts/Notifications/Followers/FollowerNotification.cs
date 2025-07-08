using Quackery.Followers;
using TMPro;
using UnityEngine;

namespace Quackery.Notifications
{

    public class FollowerNotification : Notification
    {
        [SerializeField] private TextMeshProUGUI _followerCount;

        protected override void SetInfo(NotificationInfo _)
        {

            int followerCount = FollowerServices.GetNumberOfFollowers();
            _followerCount.text = $"{followerCount.ToString("0")} followers!";

        }
    }
}
