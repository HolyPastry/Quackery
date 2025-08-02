
using System;
using KBCore.Refs;
using Quackery.Notifications;
using UnityEngine;

namespace Quackery
{
    public class NotificationApp : App
    {

        public static Action CloseRequest = delegate { };
        public static Action OpenRequest = delegate { };

        void OnEnable()
        {
            OnStartOpening += StartTheWeek;
            OnClosed += RemoveNotifications;
            CloseRequest = Close;
            OpenRequest = Open;
        }



        void OnDisable()
        {
            OnStartOpening -= StartTheWeek;
            OnClosed -= RemoveNotifications;
            CloseRequest = delegate { };
            OpenRequest = delegate { };
        }

        private void RemoveNotifications()
        {
            NotificationServices.RemoveWeeklyNotifications();
        }

        private void StartTheWeek()
        {

            NotificationServices.GenerateWeeklyNotification();
        }

    }
}
