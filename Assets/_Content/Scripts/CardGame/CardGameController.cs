using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Quackery.Clients;
using Quackery.Ratings;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;



namespace Quackery.Decks
{

    public class CardGameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private AnimatedRect _animatable;
        [SerializeField] private TextMeshProUGUI _CashInCartText;
        [SerializeField] private Transform _purseTransform;
        [SerializeField] private Transform _cartCashTransform;
        [SerializeField] private List<CardPileUI> _cardPileUIs;
        [SerializeField] private GameObject _cardSelectPanel;
        [SerializeField] private GameObject _cardBonusRealization;

        [SerializeReference] private Button _EndRoundButton;

        [SerializeField] private EndDayScreen _endDayScreen;
        [SerializeField] private EndOfRoundScreen _endRoundScreen;

        [Header("Settings")]
        [SerializeField] private int _cartSize = 2;

        private int _cashInCart = 0;
        private Vector3 _cartCashOriginalLocalPosition;
        private Client _client;
        public bool RoundInterrupted { get; private set; }
        private bool _roundInProgress;

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

        public static Func<int> GetClientCartValue = () => 0;
        public static Action InterruptRoundRequest = delegate { };

        void Awake()
        {
            _cartCashOriginalLocalPosition = _cartCashTransform.localPosition;
            _cartCashTransform.gameObject.SetActive(false);
            _gameStats = new GameStats();
        }

        void OnEnable()
        {
            CardGameContollerServices.CanCartAfford = (value) => _cashInCart >= value;
            CardGameContollerServices.ModifyCartCash = (amount) => AddCashToCart(amount);

            DeckEvents.OnCardsMovingToSelectPile += OnCardsMovingToSelectPile;
            DeckEvents.OnCashingTheCart += OnPileMovedToCart;
            DeckEvents.OnCardSelected += OnCardSelected;
            DeckEvents.OnCashingPile += OnCashingPile;
            _endDayScreen.OnCloseGame += EndTheDay;
            GetClientCartValue = () => _cashInCart;
            InterruptRoundRequest = InterruptRound;
            _EndRoundButton.onClick.AddListener(EndOfRound);
        }



        void OnDisable()
        {
            CardGameContollerServices.CanCartAfford = (value) => true;


            DeckEvents.OnCardsMovingToSelectPile -= OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected -= OnCardSelected;
            DeckEvents.OnCashingTheCart -= OnPileMovedToCart;
            DeckEvents.OnCashingPile -= OnCashingPile;
            _endDayScreen.OnCloseGame -= EndTheDay;
            GetClientCartValue = delegate { return 0; };
            InterruptRoundRequest = delegate { };
            _EndRoundButton.onClick.RemoveAllListeners();
            StopAllCoroutines();

        }



        public void Show()
        {
            ResetCart();
            _endOfDay = false;
            _gameStats.Reset();
            _endDayScreen.Hide();
            _endRoundScreen.Hide(instant: true);
            _canvas.gameObject.SetActive(true);
        }
        private void InterruptRound()
        {
            RoundInterrupted = true;
            _roundInProgress = false;
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
            _cardSelectPanel.SetActive(false);
        }

        private void OnCardsMovingToSelectPile()
        {
            _cardSelectPanel.SetActive(true);
        }

        private void OnCashingPile(CardPile pile)
        {
            if (pile == null || pile.IsEmpty) return;
            AddCashToCart(pile.TopCard.Price);
            DeckServices.DrawBackToFull();
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
                AddCashToCart(cardReward.Value);
            }


            EffectServices.Execute(Effects.EnumEffectTrigger.AfterCartCalculation, null);
            if (RoundInterrupted)
                yield break;

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

        private void AddCashToCart(int amount)
        {
            _cashInCart += amount;
            _cartCashTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 0.1f);
            _CashInCartText.text = _cashInCart.ToString("0");
        }
        private void ResetCart()
        {
            _cashInCart = 0;
            _CashInCartText.text = "0";
        }

        private void EndOfRound()
        {
            _roundInProgress = false;
        }


        public void ResetDeck()
        {
            ResetCart();
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
                    _gameStats.DayYield += _cashInCart;
                    ResetCart();
                    _gameStats.TotalRating += 5; // Assuming each round gives a fixed rating of 5

                    _cartCashTransform.localPosition = _cartCashOriginalLocalPosition;
                });

        }

        internal void StartNewRound(Client client)
        {
            DeckServices.Shuffle();
            RoundInterrupted = false;
            _client = client;
            _roundInProgress = true;
            _cartCashTransform.gameObject.SetActive(true);
            _cartCashTransform.localPosition = _cartCashOriginalLocalPosition;
            _gameStats.NumClientsServed++;
            StartCoroutine(StartRoundRoutine());

        }

        private IEnumerator StartRoundRoutine()
        {
            var cardBonus = RatingServices.GetCardBonus();

            if (cardBonus == -1)
            {
                DeckServices.SetCartSize(_cartSize + cardBonus);
                _cardBonusRealization.SetActive(true);
                yield return new WaitForSeconds(1f);
                _cardBonusRealization.SetActive(false);
            }
            else if (cardBonus >= 1)
            {
                DeckServices.SetCartSize(_cartSize);
                _cardBonusRealization.SetActive(true);
                yield return new WaitForSeconds(1f);
                _cardBonusRealization.SetActive(false);
                DeckServices.SetCartSize(_cartSize + cardBonus);
            }
            else
                DeckServices.SetCartSize(_cartSize + cardBonus);

            foreach (var effect in _client.Effects)
            {
                yield return new WaitForSeconds(0.2f);
                EffectServices.Add(effect);
                if (effect.Trigger == Effects.EnumEffectTrigger.OnRoundStart)
                    effect.Execute(null);
            }

            yield return new WaitForSeconds(0.2f);

            yield return new WaitForSeconds(1);
            DeckServices.DrawBackToFull();
        }

        internal WaitUntil WaitUntilEndOfRound()
        {
            return new WaitUntil(() => !_roundInProgress);
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
                EffectServices.Execute(Effects.EnumEffectTrigger.OnRoundEnd, pile);
            }
        }
    }
}
