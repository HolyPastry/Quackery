
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



namespace Quackery.Decks
{
    public class SlideToValidate : ValidatedMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private InputActionReference _mousePositionAction;
        [SerializeField] private float _maxSlideDistance = 420f;
        [SerializeField] private float _maxSlideDistanceY = 300f;
        [SerializeField] private bool _useDottedLine = true;

        [SerializeField, Self] private CardPileUI _cardPileUI;

        private bool _isSliding;


        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_cardPileUI.Activated ||
                 _cardPileUI.IsEmpty ||
                 _isSliding) return;

            _isSliding = true;

            DeckServices.StartPlayCardLoop(_cardPileUI.TopCard);
            StartCoroutine(SlideWithFinger());
        }

        private IEnumerator SlideWithFinger()
        {

            Vector2 originalPosition = transform.position;

            while (true)
            {
                //read the mouse position from the InputActionReference

                var mousePosition = _mousePositionAction.action.ReadValue<Vector2>();
                //   var targetPosition = Input.mousePosition;
                var distanceFromOrigin = mousePosition - originalPosition;

                distanceFromOrigin = Vector2.ClampMagnitude(distanceFromOrigin, _maxSlideDistance);
                var targetPosition = originalPosition + distanceFromOrigin;
                if (_useDottedLine && _cardPileUI.HasCartTarget)
                {
                    if (distanceFromOrigin.y > _maxSlideDistanceY)
                    {
                        OverlayCanvas.GenerateDottedLine(targetPosition, mousePosition);
                    }
                    else
                        OverlayCanvas.HideDottedLine();
                }
                else
                {
                    if (distanceFromOrigin.y > _maxSlideDistanceY)
                    {
                        DeckServices.SelectCard(_cardPileUI.Type, _cardPileUI.PileIndex);
                        ResetMovements();
                    }
                }
                transform.position = targetPosition;
                yield return null;

            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isSliding) return;

            OverlayCanvas.HideDottedLine();
            ResetMovements();
            DeckServices.StopPlayCardLoop();
        }
        private void ResetMovements()
        {
            _isSliding = false;
            StopAllCoroutines();
            // transform.localPosition = Vector3.zero;
        }
    }
}
