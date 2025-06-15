using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Quackery.Decks
{
    public class CardLogic : MonoBehaviour
    {

        [SerializeField] private Button _EndTheDayButton;
        [SerializeField] private Button _StartTheDayButton;
        [SerializeField] private TextMeshProUGUI _CashInCartText;
        [SerializeField] private Transform _purseTransform;
        [SerializeField] private Transform _cartCashTransform;

        [SerializeField] private List<CardPileUI> _cardPileUIs;

        [SerializeField] private GameObject _cardSelectPanel;
        [SerializeField] private int _cartSize = 2;
        [SerializeField] private CustomerPanelRotation _customerPanelTransform;

        private int _cashInCart = 0;
        private Vector3 _cartCashOriginalPosition;

        void Awake()
        {
            _cartCashOriginalPosition = _cartCashTransform.position;
            _cartCashTransform.gameObject.SetActive(false);
        }

        void OnEnable()
        {

            _EndTheDayButton.onClick.AddListener(EndTheDay);
            _StartTheDayButton.onClick.AddListener(() => StartNewRound());

            DeckEvents.OnCardsMovingToSelectPile += OnCardsMovingToSelectPile;
            DeckEvents.OnCachingTheCart += OnPileMovedToCart;
            DeckEvents.OnCardSelected += OnCardSelected;
        }

        void OnDisable()
        {

            _EndTheDayButton.onClick.RemoveListener(EndTheDay);
            _StartTheDayButton.onClick.RemoveAllListeners();

            DeckEvents.OnCardsMovingToSelectPile -= OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected -= OnCardSelected;
            DeckEvents.OnCachingTheCart -= OnPileMovedToCart;
            StopAllCoroutines();

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
                StartCoroutine(EndOfRoundRoutine());
            }
            else
            {
                DeckServices.DrawBackToFull();
            }
        }

        private void EndTheDay()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private IEnumerator EndOfRoundRoutine()
        {
            DialogQueueServices.QueueDialog("MeConclusion");
            DialogQueueServices.QueueDialog("ClientSuccess");
            if (_cashInCart > 0)
            {
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
            yield return DialogQueueServices.WaitUntilAllDialogEnds();

            DeckServices.DiscardCart();
            yield return new WaitForSeconds(2f);

            _customerPanelTransform.ClientExit();
            DeckServices.DiscardHand();
            yield return new WaitForSeconds(1f);

            DeckServices.ShuffleDiscardIn();
            EffectServices.CleanEffects();
            yield return new WaitForSeconds(1f);

            if (ClientServices.HasNextClient())
            {
                StartNewRound();
            }
            else
            {
                // ExitChat
            }

        }

        internal void StartNewRound()
        {


            StartCoroutine(StartRoundRoutine());
        }

        private IEnumerator StartRoundRoutine()
        {
            _customerPanelTransform.ClientEnter();
            yield return _customerPanelTransform.WaitUntilClientCameIn();
            DialogQueueServices.QueueDialog("ClientIntro");
            DialogQueueServices.QueueDialog("MeIntro");
            yield return DialogQueueServices.WaitUntilAllDialogEnds();
            _cartCashTransform.position = _cartCashOriginalPosition;
            DeckServices.SetCartSize(_cartSize);
            _cartCashTransform.gameObject.SetActive(true);
            DeckServices.Shuffle();
            yield return new WaitForSeconds(0.2f);
            DeckServices.DrawBackToFull();
        }
    }

}
