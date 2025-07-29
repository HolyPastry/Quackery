using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery.Notifications
{

    public class NotificationExpandedPanel : ValidatedMonoBehaviour, IPointerClickHandler
    {
        [SerializeField, Self] protected AnimatedRect _animatedRect;
        protected bool _inProgress;
        protected bool _clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_clicked) return;
            _clicked = true;
            _animatedRect.ZoomOut(instant: false)
                .DoComplete(() =>
                {
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                    _inProgress = false;
                    _clicked = false;
                });
        }

        public virtual IEnumerator Init(NotificationInfo info)
        {
            _inProgress = true;
            _animatedRect.ZoomIn();
            yield break;
        }

        internal virtual WaitUntil WaitUntilClosed()
        {
            return new WaitUntil(() => !_inProgress);
        }
    }
}
