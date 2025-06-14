using System;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private List<CardPileUI> _cardPileUIs;

        [SerializeField] private GameObject _cardSelectPanel;
        [SerializeField] private int _cartSize = 2;



        private int _cashInCart = 0;


        void OnEnable()
        {

            _EndTheDayButton.onClick.AddListener(EndTheDay);
            _StartTheDayButton.onClick.AddListener(() => StartRound());

            DeckEvents.OnCardsMovingToSelectPile += OnCardsMovingToSelectPile;
            DeckEvents.OnPileMovedToCart += OnPileMovedToCart;
            DeckEvents.OnCardSelected += OnCardSelected;
        }

        void OnDisable()
        {
            _EndTheDayButton.onClick.RemoveListener(EndTheDay);
            _StartTheDayButton.onClick.RemoveAllListeners();

            DeckEvents.OnCardsMovingToSelectPile -= OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected -= OnCardSelected;
            DeckEvents.OnPileMoved -= OnPileMovedToCart;
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
            }

            _CashInCartText.text = $"Cash in Cart: {_cashInCart}";


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

            if (_cashInCart > 0)
            {
                PurseServices.Modify(_cashInCart);
                _cashInCart = 0;
                _CashInCartText.text = $"Cash in Cart: {_cashInCart}";
            }
            yield return new WaitForSeconds(1f);


            DeckServices.DiscardCart();

            yield return new WaitForSeconds(1f);
            DeckServices.DiscardHand();
            yield return new WaitForSeconds(1f);
            DeckServices.ShuffleDiscardIn();

            yield return new WaitForSeconds(1f);

            StartRound();
        }

        internal void StartRound()
        {
            StartCoroutine(StartRoundRoutine());
        }

        private IEnumerator StartRoundRoutine()
        {
            DeckServices.SetCartSize(_cartSize);
            DeckServices.Shuffle();
            yield return new WaitForSeconds(2f);
            DeckServices.DrawBackToFull();
        }
    }

}
