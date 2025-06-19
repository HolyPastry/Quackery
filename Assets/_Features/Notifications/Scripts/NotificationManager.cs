using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Quackery.Notifications
{


    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _dayCountText; // Text to display the current day count
        [SerializeField] private float _notificationLifetime = 5f; // Lifetime of notifications in seconds
        [SerializeField] private GameObject _parentPanel;
        [SerializeField] private float _minimumDisplayInterval = 0.2f; // Minimum interval between notifications
        [SerializeField] private float _tapTimeOut = 0.5f; // Threshold for tap detection
        [SerializeField] private Transform _expandedPanelParent;

        [SerializeField] private List<NotificationInfo> _dailyNotificationInfos = new();
        private readonly List<Timer> _displayedNotifications = new();

        private readonly Queue<NotificationInfo> _notificationQueue = new();

        private struct Timer
        {
            public Notification instance;
            public float CreationTime;
        }

        void OnDisable()
        {
            NotificationServices.ShowNotification = delegate { };

            NotificationServices.RemoveAllNotifications = delegate { };
            NotificationServices.CloseNotification = delegate { };
            NotificationServices.ArchiveNotification = delegate { };
            NotificationServices.ShowNotificationWithDelay = delegate { };
            NotificationServices.ShowExpandedPanel = delegate { };
            NotificationServices.GenerateDailyNotification = delegate { };

        }

        void OnEnable()
        {
            NotificationServices.ShowNotification = QueueNotification;
            NotificationServices.RemoveAllNotifications = RemoveAllNotifications;
            NotificationServices.CloseNotification = CloseNotification;
            NotificationServices.ArchiveNotification = ArchiveNotification;
            NotificationServices.ShowNotificationWithDelay = (info, delay) =>
                StartCoroutine(DelayedShowNotification(info, delay));
            NotificationServices.ShowExpandedPanel = ShowExpandedPanel;
            NotificationServices.GenerateDailyNotification = GenerateDailyNotification;

        }

        private void GenerateDailyNotification()
        {
            _dayCountText.text = $"Day {CalendarServices.Today()}";
            foreach (var info in _dailyNotificationInfos)
            {
                ShowNotification(info);
            }
        }

        private void ShowExpandedPanel(NotificationInfo info)
        {

            NotificationExpandedPanel panel
                = Instantiate(info.ExpandedPanelPrefab, _expandedPanelParent);

        }

        private IEnumerator DelayedShowNotification(NotificationInfo info, float delay)
        {
            yield return new WaitForSeconds(delay);
            ShowNotification(info);
        }

        void Start()
        {
            StartCoroutine(StaggeredDisplayRoutine());
        }

        private IEnumerator StaggeredDisplayRoutine()
        {
            var waitForSeconds = new WaitForSeconds(_minimumDisplayInterval);
            while (true)
            {
                yield return waitForSeconds;

                if (_notificationQueue.Count == 0) continue;

                var info = _notificationQueue.Dequeue();
                ShowNotification(info);

            }
        }

        private void ArchiveNotification(NotificationInfo info)
        {
            var notification = _displayedNotifications.Find(timer => timer.instance.Info == info);
            if (notification.instance != null)
            {
                _displayedNotifications.Remove(notification);
                notification.instance.Archive();
            }
            else
            {
                Debug.LogWarning("Notification not found to archive.");
            }
        }

        private void CloseNotification(NotificationInfo info)
        {
            var notification = _displayedNotifications.Find(timer => timer.instance.Info == info);
            if (notification.instance != null)
            {
                _displayedNotifications.Remove(notification);
                notification.instance.Bin();

            }
            else
            {
                Debug.LogWarning("Notification not found to close.");
            }
        }

        private void QueueNotification(NotificationInfo info)
        {
            _notificationQueue.Enqueue(info);
        }

        private void ShowNotification(NotificationInfo info)
        {
            Notification notification = Instantiate(info.Prefab, _parentPanel.transform);

            notification.transform.SetAsLastSibling();
            notification.Show(info, _tapTimeOut);
            if (info.IsPersistent)
            {
                _displayedNotifications.Add(new Timer
                {
                    instance = notification,
                    CreationTime = -1
                });
            }
            else
            {
                _displayedNotifications.Add(new Timer
                {
                    instance = notification,
                    CreationTime = Time.time
                });
            }
        }

        private void RemoveAllNotifications()
        {

            while (_parentPanel.transform.childCount > 0)
            {

                var child = _parentPanel.transform.GetChild(0);
                child.gameObject.SetActive(false);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        private void ShowAllNotifications()
        {
            _parentPanel.SetActive(true);
        }

        void Update()
        {
            int i = 0;
            while (i < _displayedNotifications.Count)
            {
                var timer = _displayedNotifications[i];
                if (timer.CreationTime < 0)
                {
                    // Persistent notification, do not remove
                    i++;
                    continue;
                }

                if (Time.time - timer.CreationTime > _notificationLifetime)
                {
                    _displayedNotifications.RemoveAt(i);
                    if (timer.instance != null)
                        timer.instance.Bin();
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
