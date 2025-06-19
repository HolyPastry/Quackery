
using DG.Tweening;
using Holypastry.Bakery;

using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Notifications
{
    public abstract class Notification : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [SerializeField] private RectTransform _panel;
        public NotificationInfo Info { get; private set; }

        private CountdownTimer _tapTimer;
        private bool _grabbed;
        private Vector2 _pointerDownPosition;

        void Awake()
        {
            _panel.gameObject.SetActive(false);
        }

        void Update()
        {
            _tapTimer.Tick(Time.deltaTime);
        }

        internal void Show(NotificationInfo info, float tapTimeOut)
        {
            _tapTimer = new(tapTimeOut);
            Info = info;
            SetInfo(info);
            _panel.localScale = Vector3.one;
            _panel.anchoredPosition = new Vector2(Screen.width, 0);
            _panel.gameObject.SetActive(true);
            _panel.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack);
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
        internal void Archive()
        {
            var destination = Screen.width;
            if (_panel.anchoredPosition.x < 0)
                destination = -Screen.width;

            _panel.DOAnchorPosX(destination, 0.5f).OnComplete(() =>
            {
                _panel.gameObject.SetActive(false);
                Destroy(gameObject);
            });
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _grabbed = true;
            _pointerDownPosition = eventData.position;

            _tapTimer.Start();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _grabbed = false;

            if (!_tapTimer.IsRunning) return;
            // NotificationServices.ShowExpandedPanel(Info);
            //NotificationServices.CloseNotification?.Invoke(Info);
            Info.OnTapped?.Invoke(Info);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!_grabbed) return;

            float deltaX = eventData.position.x - _pointerDownPosition.x;
            _panel.anchoredPosition += new Vector2(deltaX, 0);
            _pointerDownPosition = eventData.position;
            if (_panel.anchoredPosition.x < -Screen.width / 4 ||
                 _panel.anchoredPosition.x > Screen.width / 4)
            {
                _tapTimer.Stop();
                _grabbed = false;
                NotificationServices.ArchiveNotification?.Invoke(Info);

            }
        }
    }
}
