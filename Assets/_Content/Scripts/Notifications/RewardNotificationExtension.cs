
using System;
using System.Collections.Generic;
using Ink.Runtime;
using Quackery.Clients;
using Quackery.Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Quackery
{
    public class RewardNotificationExtension : MonoBehaviour
    {
        [SerializeField] private Notification _rewardNotificationPrefab;

        public static Action<Client> Push = delegate { };

        void OnEnable()
        {
            Push += QueueRewardNotification;
        }
        void OnDisable()
        {
            Push -= QueueRewardNotification;
        }

        private void QueueRewardNotification(Client client)
        {
            RewardNotificationInfo notification = new()
            {
                Prefab = _rewardNotificationPrefab,
                Portrait = client.Portrait,
                ReviewText = "Amazing service!, Thank You!",
                Rating = UnityEngine.Random.Range(4, 6)
            };
            NotificationServices.ShowNotificationWithDelay(notification, UnityEngine.Random.Range(5f, 8f));
        }
    }

}
