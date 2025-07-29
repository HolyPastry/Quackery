using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using TMPro;
using UnityEngine;

namespace Quackery.Notifications
{
    public class NotificationManager : Service
    {
        [SerializeField] private TextMeshProUGUI _dayCountText; // Text to display the current day count
                                                                // [SerializeField] private float _notificationLifetime = 5f; // Lifetime of notifications in seconds
        [SerializeField] private GameObject _parentPanel;
        [SerializeField] private float _minimumDisplayInterval = 0.2f; // Minimum interval between notifications

        [SerializeField] private Transform _expandedPanelParent;
        [SerializeField] private SlidingButton _slideToUnlock;
        private List<NotificationExtension> _extensions = new();
        private bool _initialized;
        private readonly List<Timer> _displayedNotifications = new();
        private readonly Queue<NotificationInfo> _notificationQueue = new();

        private bool IsExpandedPanelOn => _expandedPanelParent.gameObject.activeSelf;

        public Action OnClosed = delegate { };

        private struct Timer
        {
            public Notification instance;
            public float CreationTime;
        }

        void Awake()
        {
            GetComponentsInChildren(true, _extensions);
        }

        void OnDisable()
        {
            NotificationServices.ShowNotification = delegate { };

            NotificationServices.RemoveWeeklyNotifications = delegate { };

            NotificationServices.ArchiveNotification = delegate { };

            NotificationServices.ShowExpandedPanel = delegate { };
            NotificationServices.GenerateWeeklyNotification = delegate { };

            NotificationServices.WaitUntilReady = () => new WaitUntil(() => true);

            _slideToUnlock.OnFinishSliding -= NotificationApp.CloseRequest;

            StopAllCoroutines();
        }

        void OnEnable()
        {
            NotificationServices.ShowNotification = QueueNotification;
            NotificationServices.RemoveWeeklyNotifications = RemoveWeeklyNotifications;

            NotificationServices.ArchiveNotification = ArchiveNotification;

            NotificationServices.ShowExpandedPanel = ShowExpandedPanel;
            NotificationServices.GenerateWeeklyNotification = GenerateWeeklyNotification;
            NotificationServices.WaitUntilReady = () => WaitUntilReady;

            _slideToUnlock.OnFinishSliding += NotificationApp.CloseRequest;

            if (_initialized)
                StartCoroutine(StaggeredDisplayRoutine());

        }



        private void GenerateWeeklyNotification()
        {
            _dayCountText.text = $"Week {CalendarServices.Today()}";

            foreach (var extension in _extensions)
            {
                extension.GenerateDailyNotification();
            }
        }

        private void ShowExpandedPanel(NotificationInfo info)
        {
            if (IsExpandedPanelOn) return;
            StartCoroutine(ExpandedPanelRoutine(info));
        }

        private IEnumerator ExpandedPanelRoutine(NotificationInfo info)
        {
            _expandedPanelParent.gameObject.SetActive(true);
            foreach (var extension in _extensions)
            {
                if (!extension.MatchType(info)) continue;
                yield return StartCoroutine(extension.ExpandedPanelRoutine(info, _expandedPanelParent));
                break;
            }
            _expandedPanelParent.gameObject.SetActive(false);
        }

        private IEnumerator DelayedShowNotification(NotificationInfo info, float delay)
        {
            yield return new WaitForSeconds(delay);
            ShowNotification(info);
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            StartCoroutine(StaggeredDisplayRoutine());
            _initialized = true;
        }

        private IEnumerator StaggeredDisplayRoutine()
        {
            _slideToUnlock.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            var waitForSeconds = new WaitForSeconds(_minimumDisplayInterval);
            while (true)
            {
                if (_notificationQueue.Count == 0)
                {
                    yield return waitForSeconds;
                    continue;
                }

                var info = _notificationQueue.Dequeue();
                ShowNotification(info);
                yield return waitForSeconds;
                if (_notificationQueue.Count == 0)
                    break;
            }
            _slideToUnlock.gameObject.SetActive(true);
        }

        private void ArchiveNotification(NotificationInfo info)
        {
            var notification = _displayedNotifications.Find(timer => timer.instance.Info == info);
            if (notification.instance != null)
            {
                _displayedNotifications.Remove(notification);
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
            Notification notification = Instantiate(info.Data.Prefab, _parentPanel.transform);

            notification.transform.SetAsLastSibling();
            notification.Show(info);
            if (info.Data.IsPersistent)
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

        private void RemoveWeeklyNotifications()
        {
            int permanentCount = 0;
            while (_parentPanel.transform.childCount > permanentCount)
            {
                // Notification notification = _parentPanel.transform.GetChild(0).GetComponent<Notification>();
                // if (notification.IsPersistent)
                // {
                //     permanentCount++;
                //     continue;
                // }
                var child = _parentPanel.transform.GetChild(0);
                child.gameObject.SetActive(false);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
            _displayedNotifications.Clear();
        }

        // private void ShowAllNotifications()
        // {
        //     _parentPanel.SetActive(true);
        // }

        // void Update()
        // {
        //     int i = 0;
        //     while (i < _displayedNotifications.Count)
        //     {
        //         var timer = _displayedNotifications[i];
        //         if (timer.CreationTime < 0)
        //         {
        //             // Persistent notification, do not remove
        //             i++;
        //             continue;
        //         }

        //         if (Time.time - timer.CreationTime > _notificationLifetime)
        //         {
        //             _displayedNotifications.RemoveAt(i);
        //             if (timer.instance != null)
        //                 timer.instance.Bin();
        //         }
        //         else
        //         {
        //             i++;
        //         }
        //     }
        // }
    }
}
