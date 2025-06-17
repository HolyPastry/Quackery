using System;

using KBCore.Refs;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class InstaPost : ValidatedMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Self] private AnimatedRect _animatedRect;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Image _banner;

        public event Action<InstaReward> OnPostClicked;

        public InstaReward Data { get; private set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //noop
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //noop
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //noop
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPostClicked?.Invoke(Data);
        }

        public void AddPost(InstaReward data)
        {
            gameObject.SetActive(true);
            _title.text = data.name;
            _banner.sprite = data.Banner;
            Data = data;
            _animatedRect.SlideIn(Direction.Right);
        }
        public void Hide()
        {
            _animatedRect.ZoomOut(true);
            gameObject.SetActive(false);
        }
    }
}
