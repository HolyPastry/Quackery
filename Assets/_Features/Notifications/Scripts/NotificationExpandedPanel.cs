using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery.Notifications
{
    public abstract class NotificationExpandedPanel : ValidatedMonoBehaviour, IPointerClickHandler
    {
        [SerializeField, Self] private AnimatedRect _animatedRect;

        void OnEnable()
        {
            _animatedRect.ZoomIn();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _animatedRect.ZoomOut(instant: false)
                .DoComplete(() =>
                {
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                });

        }
    }
}
