using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery
{
    public class EndOfRoundScreen : ValidatedMonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI _clientResult;
        [SerializeField, Self] private AnimatedRect _animatedRect;
        [SerializeField] private Image _portraitImage;
        [SerializeField] private TextMeshProUGUI _reviewText;
        [SerializeField] private List<GameObject> _ratingStars;
        public void Show(Client client, bool success)
        {
            gameObject.SetActive(true);
            if (success)
                _clientResult.text = "And Another Satisfied Customer!";
            else
                _clientResult.text = "The Client Left Without Paying!";

            _portraitImage.sprite = client.Portrait;
            _reviewText.text = client.LastReviewText;
            for (int i = 0; i < _ratingStars.Count; i++)
                _ratingStars[i].SetActive(i < client.LastRating);
            _animatedRect.SlideIn(Direction.Right);
        }

        public void Hide(bool instant)
        {
            if (instant)
                gameObject.SetActive(false);
            else
                _animatedRect.ZoomOut(false)
                              .DoComplete(() => gameObject.SetActive(false));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Hide(instant: false);
        }
    }
}
