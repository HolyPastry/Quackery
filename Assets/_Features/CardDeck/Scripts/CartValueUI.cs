
using System;
using DG.Tweening;
using UnityEngine;

namespace Quackery.Decks
{
    public class CartValueUI : MonoBehaviour
    {
        [SerializeField] protected TMPro.TextMeshProUGUI _cartValueText;
        void OnEnable()
        {
            CartEvents.OnCartValueChanged += UpdateCartValue;
        }

        void OnDisable()
        {
            CartEvents.OnCartValueChanged -= UpdateCartValue;
        }

        protected virtual void UpdateCartValue()
        {
            _cartValueText.text = $"<sprite name=Coin> {CartServices.GetCartValue()}";
        }
        public void Show()
        {
            gameObject.SetActive(true);

            UpdateCartValue();
            ResetPosition();
        }

        public void MoveTo(Transform target, Action onComplete)
        {
            transform.DOMove(target.position, 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    Hide();
                });
        }
        public void ResetPosition()
        {
            transform.localPosition = Vector3.zero;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ResetPosition();
        }
    }
}
