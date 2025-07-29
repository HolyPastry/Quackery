
using System;
using DG.Tweening;
using UnityEngine;



namespace Quackery.Notifications
{
    public class Notification : SlidingButton
    {
        public NotificationInfo Info { get; private set; }
        public bool IsPersistent => Info.Data.IsPersistent;

        public event Action OnArchived = delegate { };

        protected override void Awake()
        {
            base.Awake();
            _panel.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            OnFinishSliding += ArchiveNotification;
            OnTapped += OpenExpandedPanel;

        }
        void OnDisable()
        {
            OnFinishSliding -= ArchiveNotification;
            OnTapped -= OpenExpandedPanel;
        }

        private void OpenExpandedPanel()
        {
            NotificationServices.ShowExpandedPanel(Info);
        }


        private void ArchiveNotification()
        {
            NotificationServices.ArchiveNotification(Info);
        }

        internal void Show(NotificationInfo info)
        {
            Info = info;
            SetInfo(info);
            _panel.localScale = Vector3.one;
            _panel.anchoredPosition = new Vector2(Screen.width, 0);
            _panel.gameObject.SetActive(true);
            _panel.DOAnchorPosX(0, 0.8f).SetEase(Ease.OutQuint);
        }

        protected virtual void SetInfo(NotificationInfo info)
        { }

        internal void Bin()
        {
            _panel.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
           {
               _panel.gameObject.SetActive(false);
               Destroy(gameObject);
           });
        }

    }
}
