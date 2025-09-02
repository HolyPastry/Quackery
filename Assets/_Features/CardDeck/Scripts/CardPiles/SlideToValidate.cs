
using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



namespace Quackery.Decks
{
    public class SlideToValidate : ValidatedMonoBehaviour
    {
        [SerializeField] private InputActionReference _mousePositionAction;
        [SerializeField] private float _maxSlideDistance = 420f;
        [SerializeField] private float _maxSlideDistanceY = 300f;
        [SerializeField] private bool _useDottedLine = true;
        [SerializeField] private float _slideStartThreshold = 10f;

        [SerializeField, Self] private CardPile _cardPileUI;

        private bool _isSliding;
        private bool _isSelected;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_cardPileUI.IsEmpty) return;

            StartCoroutine(SlideWithFinger());
        }

        private IEnumerator SlideWithFinger()
        {

            DeckServices.StartPlayCardLoop(_cardPileUI.TopCard);

            Vector2 originalPosition = transform.position;

            while (true)
            {
                yield return null;
                var mousePosition = _mousePositionAction.action.ReadValue<Vector2>();

                var distanceFromOrigin = mousePosition - originalPosition;
                distanceFromOrigin = Vector2.ClampMagnitude(distanceFromOrigin, _maxSlideDistance);

                _isSliding = distanceFromOrigin.sqrMagnitude > _slideStartThreshold * _slideStartThreshold;

                if (!_isSliding)
                {
                    // HandController.CardPileControlRequest?.Invoke(null);
                    continue;
                }

                //HandController.CardPileControlRequest?.Invoke(_cardPileUI);

                var targetPosition = originalPosition + distanceFromOrigin;
                if (_useDottedLine && _cardPileUI.HasCartTarget)
                {
                    // if (distanceFromOrigin.y > _maxSlideDistanceY)
                    //     DottedLine.GenerateDottedLine(targetPosition, mousePosition);
                    // else
                    //     DottedLine.HideDottedLine();
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


            }
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isSliding)
            {
                ResetMovements();
                DottedLine.HideDottedLine();
                DeckServices.StopPlayCardLoop();
                return;
            }
            if (!_isSelected)
            {
                _isSelected = true;
                DeckServices.StartPlayCardLoop(_cardPileUI.TopCard);
                return;
            }
            if (_isSelected)
            {
                _isSelected = false;
                DeckServices.StopPlayCardLoop();
            }
        }
        private void ResetMovements()
        {
            _isSliding = false;
            StopAllCoroutines();
            //  HandController.CardPileControlRequest?.Invoke(null);

        }
    }
}
