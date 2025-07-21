
using System;
using DG.Tweening;
using UnityEngine;

namespace Quackery.Decks
{
    public class CartValueUI : MonoBehaviour
    {
        [SerializeField] protected TMPro.TextMeshProUGUI _cartValueText;

        public static Func<Transform> Transform = () => null;


        void OnEnable()
        {
            CartEvents.OnValueChanged += UpdateCartValue;
            Transform = () => transform;
        }

        void OnDisable()
        {
            CartEvents.OnValueChanged -= UpdateCartValue;
            Transform = () => null;
        }

        protected virtual void UpdateCartValue()
        {
            _cartValueText.text = $"<sprite name=Coin> {CartServices.GetValue()}";
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
