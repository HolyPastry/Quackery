using System;
using System.Collections;
using DG.Tweening;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Decks
{
    internal class EndDayScreen : ValidatedMonoBehaviour
    {

        [SerializeField, Self] private AnimatedRect _animatable;
        [Header("UI Elements")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private AnimatedText _dayYieldText;
        [SerializeField] private AnimatedText _numClientsServedText;
        [SerializeField] private AnimatedText _averageRatingText;
        [SerializeField] private AnimatedText _averageCashPerClientText;
        [SerializeField] private AnimatedText _numQuacksText;
        [Header("Animation  Settings")]
        [SerializeField] private float _delaybetweenText = 0.2f;

        public event Action OnCloseGame;

        internal void Init(CardGameController.GameStats gameStats)
        {
            _dayYieldText.Text = gameStats.DayYield.ToString();
            _numClientsServedText.Text = gameStats.NumClientsServed.ToString();
            _averageRatingText.Text = gameStats.AverageRating.ToString();
            _averageCashPerClientText.Text = gameStats.AverageCashPerClient.ToString();
            _numQuacksText.Text = gameStats.NumQuacks.ToString();

        }
        void OnDisable()
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        internal void Show()
        {
            gameObject.SetActive(true);
            _animatable.ZoomIn();
            StartCoroutine(ShowCoroutine());

        }
        public void Hide()
        {
            _animatable.ZoomOut(instant: true);
        }

        private IEnumerator ShowCoroutine()
        {

            yield return _animatable.WaitForAnimation();
            _closeButton.onClick.AddListener(CloseGame);
            _numClientsServedText.AnimatedRect.SlideIn(Direction.Right);
            yield return new WaitForSeconds(_delaybetweenText);
            _averageRatingText.AnimatedRect.SlideIn(Direction.Right);
            yield return new WaitForSeconds(_delaybetweenText);
            _numQuacksText.AnimatedRect.SlideIn(Direction.Right);

            yield return new WaitForSeconds(_delaybetweenText);
            _averageCashPerClientText.AnimatedRect.SlideIn(Direction.Right);
            yield return new WaitForSeconds(_delaybetweenText);
            _dayYieldText.AnimatedRect.SlideIn(Direction.Right);

        }
        private void CloseGame()
        {
            _closeButton.onClick.RemoveListener(CloseGame);
            OnCloseGame?.Invoke();
        }
    }

}
