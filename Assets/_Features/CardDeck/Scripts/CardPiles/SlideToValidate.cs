
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Quackery.Decks
{
    public class SlideToValidate : ValidatedMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, Self] private CardPileUI _cardPileUI;
        private Vector3 _originalPosition;
        private bool _sliding;

        void Awake()
        {
            _originalPosition = transform.localPosition;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_cardPileUI.Activated || _cardPileUI.IsEmpty) return;

            _sliding = true;
            StartCoroutine(SlideWithFinger());
        }

        private IEnumerator SlideWithFinger()
        {
            var mouseStartPosition = Input.mousePosition;
            _originalPosition = transform.localPosition;

            while (true)
            {
                var deltaY = Input.mousePosition.y - mouseStartPosition.y;
                deltaY = Mathf.Clamp(deltaY, 0, 400);
                if (deltaY > 370)
                {
                    ValidateCard();
                    yield break;
                }
                var targetPosition = new Vector2(_originalPosition.x, _originalPosition.y + deltaY);

                transform.localPosition =
                    Vector2.Lerp(transform.localPosition,
                                targetPosition,
                                Time.deltaTime * 10f);

                yield return null;
            }
        }

        private void ValidateCard()
        {
            DeckServices.PileClicked(_cardPileUI.Type);
            ResetMovements();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_sliding) return;
            ResetMovements();
        }
        private void ResetMovements()
        {
            _sliding = false;
            StopAllCoroutines();
            transform.localPosition = _originalPosition;
        }
    }
}
