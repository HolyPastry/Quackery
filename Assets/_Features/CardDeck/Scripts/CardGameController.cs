using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Quackery.Decks
{


    public class CardGameController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Animatable _animatable;
        // [SerializeField] private Button _EndTheDayButton;
        // [SerializeField] private Button _StartTheDayButton;
        [SerializeField] private TextMeshProUGUI _CashInCartText;
        [SerializeField] private Transform _purseTransform;
        [SerializeField] private Transform _cartCashTransform;

        [SerializeField] private List<CardPileUI> _cardPileUIs;

        [SerializeField] private GameObject _cardSelectPanel;
        [SerializeField] private int _cartSize = 2;

        private int _cashInCart = 0;
        private Vector3 _cartCashOriginalPosition;
        private bool _roundInProgress;

        void Awake()
        {
            _cartCashOriginalPosition = _cartCashTransform.position;
            _cartCashTransform.gameObject.SetActive(false);
        }

        void OnEnable()
        {

            DeckEvents.OnCardsMovingToSelectPile += OnCardsMovingToSelectPile;
            DeckEvents.OnCachingTheCart += OnPileMovedToCart;
            DeckEvents.OnCardSelected += OnCardSelected;
        }

        void OnDisable()
        {


            DeckEvents.OnCardsMovingToSelectPile -= OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected -= OnCardSelected;
            DeckEvents.OnCachingTheCart -= OnPileMovedToCart;
            StopAllCoroutines();

        }

        public void Show()
        {
            _canvas.gameObject.SetActive(true);
            _animatable.SlideIn();
        }

        public void Hide()
        {
            _animatable.SlideOut().OnComplete(() =>
            {
                _canvas.gameObject.SetActive(false);
            });
        }
        private void OnCardSelected(Card card, List<Card> list)
        {
            StartCoroutine(DelayedSwitchOffSelectPanel());
        }

        private IEnumerator DelayedSwitchOffSelectPanel()
        {
            yield return null;
            _cardSelectPanel.SetActive(false);
        }

        private void OnCardsMovingToSelectPile()
        {
            _cardSelectPanel.SetActive(true);
        }

        private void OnPileMovedToCart(EnumPileType type)
        {
            StartCoroutine(CartRewardRoutine(type));
        }

        private IEnumerator CartRewardRoutine(EnumPileType type)
        {
            yield return new WaitForSeconds(0.5f);

            List<CardReward> cardRewards = DeckServices.GetPileRewards(type);
            //  _cashInCart += DeckServices.EvaluatePileValue(type);
            var pileUI = _cardPileUIs.Find(p => p.Type == type);
            foreach (CardReward cardReward in cardRewards)
            {
                pileUI.ShowReward(cardReward);
                yield return new WaitForSeconds(0.8f);
                _cashInCart += cardReward.Value;
                _cartCashTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 0.1f);
            }

            _CashInCartText.text = _cashInCart.ToString("0");


            yield return new WaitForSeconds(0.5f);
            if (DeckServices.IsCartFull())
            {
                EndOfRound();
            }
            else
            {
                DeckServices.DrawBackToFull();
            }
        }

        private void EndOfRound() => _roundInProgress = false;


        public void ResetDeck()
        {
            DeckServices.DiscardCart();
            DeckServices.DiscardHand();
            DeckServices.ShuffleDiscardIn();
        }

        public void TransfertCartToPurse()
        {
            if (_cashInCart <= 0) return;

            _cartCashTransform.DOMove(_purseTransform.position, 0.5f)
                .OnComplete(() =>
                {
                    PurseServices.Modify(_cashInCart);
                    _purseTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 0.1f);
                    _cartCashTransform.gameObject.SetActive(false);
                    _cashInCart = 0;
                    _CashInCartText.text = "0";
                    _cartCashTransform.position = _cartCashOriginalPosition;
                });

        }

        internal void StartNewRound()
        {
            _roundInProgress = true;
            _cartCashTransform.gameObject.SetActive(true);
            _cartCashTransform.position = _cartCashOriginalPosition;
            StartCoroutine(StartRoundRoutine());
        }

        private IEnumerator StartRoundRoutine()
        {
            DeckServices.SetCartSize(_cartSize);
            DeckServices.Shuffle();
            yield return new WaitForSeconds(0.2f);
            DeckServices.DrawBackToFull();

        }

        internal WaitUntil WaitUntilEndOfRound()
        {
            return new WaitUntil(() => !_roundInProgress);
        }
    }

}
