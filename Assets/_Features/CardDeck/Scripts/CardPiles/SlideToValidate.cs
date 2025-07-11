
using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;


namespace Quackery.Decks
{
    public class SlideToValidate : ValidatedMonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        [SerializeField] private float _maxSlideDistance = 420f;
        [SerializeField] private float _maxSlideDistanceY = 300f;

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

            var originalPosition = transform.position;

            while (true)
            {

                var targetPosition = Input.mousePosition;
                var distanceFromOrigin = targetPosition - originalPosition;

                distanceFromOrigin = Vector2.ClampMagnitude(distanceFromOrigin, _maxSlideDistance);
                targetPosition = originalPosition + distanceFromOrigin;
                if (distanceFromOrigin.y > _maxSlideDistanceY)
                    OverlayCanvas.GenerateDottedLine(targetPosition, Input.mousePosition);
                else
                    OverlayCanvas.HideDottedLine();

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
