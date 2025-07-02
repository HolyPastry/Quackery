
using System;
using DG.Tweening;
using UnityEngine;

namespace Quackery.Decks
{
    public class CartValueUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _cartValueText;
        void OnEnable()
        {
            CartEvents.OnCartValueChanged += UpdateCartValue;
        }

        void OnDisable()
        {
            CartEvents.OnCartValueChanged -= UpdateCartValue;
        }

        private void UpdateCartValue(int value)
        {
            _cartValueText.text = value.ToString();
        }
        public void Show()
        {

            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;
            UpdateCartValue(CartServices.GetCartValue());
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
