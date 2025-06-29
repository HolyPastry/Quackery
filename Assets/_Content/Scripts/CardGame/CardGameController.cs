using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Clients;
using Quackery.Ratings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace Quackery.Decks
{

    public class CardGameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private AnimatedRect _animatable;
        [SerializeField] private CartValueUI _cartValue;
        [SerializeField] private Transform _purseTransform;

        [SerializeField] private CardPool _cardSelectPanel;
        [SerializeField] private GameObject _cardBonusRealization;

        [SerializeReference] private Button _EndRoundButton;

        [SerializeField] private EndDayScreen _endDayScreen;
        [SerializeField] private EndOfRoundScreen _endRoundScreen;

        private Client _client;
        public bool RoundInterrupted { get; private set; }
        private bool _endButtonPressed;

        public class GameStats
        {
            public int NumClientsServed;
            public int AverageRating
                    => NumClientsServed > 0 ? TotalRating / NumClientsServed : 0;
            public int TotalRating;
            public int AverageCashPerClient
                => NumClientsServed > 0 ? DayYield / NumClientsServed : 0;
            public int DayYield;
            public int NumQuacks;

            public void Reset()
            {
                NumClientsServed = 0;
                TotalRating = 0;
                DayYield = 0;
                NumQuacks = 0;
            }
        }
        private GameStats _gameStats;
        private bool _endOfDay;
        public static Action InterruptRoundRequest = delegate { };
        private bool _endOfRound;


        void Awake()
        {
            _cartValue.Hide();
            _gameStats = new GameStats();
            _EndRoundButton.interactable = false;
        }

        void OnEnable()
        {

            DeckEvents.OnCardsMovingToSelectPile += OnCardsMovingToSelectPile;
            //      DeckEvents.OnCalculatingCartPile += OnPileMovedToCart;
            DeckEvents.OnCardSelected += OnCardSelected;
            //  DeckEvents.OnCashingPile += OnCashingPile;
            _endDayScreen.OnCloseGame += EndTheDay;
            InterruptRoundRequest = () => RoundInterrupted = true;
            _EndRoundButton.onClick.AddListener(() => _endButtonPressed = true);
        }



        void OnDisable()
        {

            DeckEvents.OnCardsMovingToSelectPile -= OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected -= OnCardSelected;

            _endDayScreen.OnCloseGame -= EndTheDay;
            InterruptRoundRequest = delegate { };
            _EndRoundButton.onClick.RemoveAllListeners();
            StopAllCoroutines();

        }

        public void Show()
        {
            CartServices.SetCartValue(0);
            _endOfDay = false;
            _gameStats.Reset();
            _endDayScreen.Hide();
            _endRoundScreen.Hide(instant: true);
            _canvas.gameObject.SetActive(true);
            _animatable.SlideIn(Direction.Right);
        }

        private void EndTheDay() => _endOfDay = true;

        public void Hide()
        {
            _animatable.SlideOut(Direction.Right).DoComplete(() =>
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
            _cardSelectPanel.Hide();
        }

        private void OnCardsMovingToSelectPile()
        {
            _cardSelectPanel.Show();
        }

        // private void OnCashingPile(CardPile pile)
        // {
        //     if (pile == null || pile.IsEmpty) return;
        //     AddCashToCart(pile.TopCard.Price);
        //     DeckServices.DrawBackToFull();
        // }

        private void OnPileMovedToCart(EnumCardPile type, int index)
        {
            StartCoroutine(CartRewardRoutine(type, index));
        }

        private IEnumerator CalculateCartRoutine()
        {
            yield return new WaitForSeconds(0.5f);


            // foreach (var pileUI in _cardPileUIs)
            // {
            //     yield return StartCoroutine(CartRewardRoutine(pileUI.Type, pileUI.PileIndex));
            yield return new WaitForSeconds(0.2f);
            // }
        }

        private IEnumerator CartRewardRoutine(EnumCardPile type, int index)
        {
            yield return null;
            // //  _cashInCart += DeckServices.EvaluatePileValue(type);
            // var pileUI = _cardPileUIs.Find(p => p.Type == type && p.PileIndex == index);
            // foreach (CardReward cardReward in cardRewards)
            // {
            //     pileUI.ShowReward(cardReward);
            //     yield return new WaitForSeconds(0.8f);
            //     AddCashToCart(cardReward.Value);
            // }
            // EffectServices.Execute(Effects.EnumEffectTrigger.AfterCartCalculation, null);
        }

        public void TransfertCartToPurse()
        {
            var cashInCart = CartServices.GetCartValue();
            if (cashInCart <= 0) return;
            _cartValue.MoveTo(_purseTransform, () =>
            {
                PurseServices.Modify(cashInCart);
                _purseTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 0.1f);

                _gameStats.DayYield += cashInCart;
                CartServices.SetCartValue(0);
                _gameStats.TotalRating += 5; // Assuming each round gives a fixed rating of 5
            });
        }

        internal void StartNewRound(Client client)
        {
            _EndRoundButton.interactable = false;
            RoundInterrupted = false;
            _endButtonPressed = false;
            _endOfRound = false;

            _client = client;

            _cartValue.Show();
            _gameStats.NumClientsServed++;
            StartCoroutine(RoundRoutine());

        }

        private IEnumerator RoundRoutine()
        {
            yield return StartCoroutine(ApplyRatingBonus());

            yield return StartCoroutine(ApplyClientEffects());
            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                yield return DeckServices.DrawBackToFull();

                DeckServices.ActivateTableCards();

                SetEndRoundButtonInteractable(true);

                yield return new WaitUntil(() => DeckServices.CardPlayed() ||
                                                 _endButtonPressed ||
                                                 RoundInterrupted);
                SetEndRoundButtonInteractable(false);


                if (RoundInterrupted)
                {
                    _endOfRound = true;
                    yield break;
                }

                if (_endButtonPressed == true)
                    break;
                CartServices.CalculateCart();

            }
            yield return DeckServices.DiscardHand();

            //EffectServices.Execute(Effects.EnumEffectTrigger.OnRoundEnd, null);
            _endOfRound = true;
        }

        private void SetEndRoundButtonInteractable(bool isOn)
        {
            _EndRoundButton.interactable = isOn;
            if (isOn && DeckServices.NoPlayableCards())
                _EndRoundButton.GetComponent<Image>().color = Color.green;
            else
                _EndRoundButton.GetComponent<Image>().color = Color.white;
        }

        private IEnumerator ApplyClientEffects()
        {
            foreach (var effect in _client.Effects)
            {
                yield return new WaitForSeconds(0.2f);
                EffectServices.AddStatus(effect);
                if (effect.Trigger == Effects.EnumEffectTrigger.OnRoundStart)
                    effect.Execute(null);
            }
        }

        private IEnumerator ApplyRatingBonus()
        {
            var cardBonus = RatingServices.GetCardBonus();

            if (cardBonus == -1)
            {
                CartServices.SetRatingCartSizeModifier(cardBonus);
                _cardBonusRealization.SetActive(true);

                yield return new WaitForSeconds(1f);

                _cardBonusRealization.SetActive(false);
            }
            else if (cardBonus >= 1)
            {
                CartServices.SetRatingCartSizeModifier(0);
                _cardBonusRealization.SetActive(true);
                yield return new WaitForSeconds(1f);
                _cardBonusRealization.SetActive(false);
                CartServices.SetRatingCartSizeModifier(cardBonus);
            }
            else
                CartServices.SetRatingCartSizeModifier(cardBonus);
        }

        internal WaitUntil WaitUntilEndOfRound()
        {
            return new WaitUntil(() => _endOfRound);
        }

        public void ShowEndRoundScreen(bool success)
        {
            _endRoundScreen.Show(_client, success);
        }
        public void HideEndRoundScreen()
        {
            _endRoundScreen.Hide(instant: false);
        }

        internal void ShowEndDayScreen()
        {
            _endDayScreen.Init(_gameStats);
            _endDayScreen.Show();
        }

        internal object WaitUntilEndOfDayValidated()
        {
            return new WaitUntil(() => _endOfDay);
        }

        internal WaitUntil WaitUntilEndOfRoundScreenClosed()
        {
            float time = Time.time;

            return new WaitUntil(() =>
                Time.time - time > 10f ||
                !_endRoundScreen.gameObject.activeSelf);
        }

        internal void ApplyEndRoundEffects()
        {
            var tablepiles = DeckServices.GetTablePile();
            foreach (var pile in tablepiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                //EffectServices.Execute(Effects.EnumEffectTrigger.OnRoundEnd, pile.TopCard);
            }
        }
    }
}
