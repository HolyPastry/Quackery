using System;
using System.Collections;
using System.Collections.Generic;

using Quackery.Clients;
using Quackery.GameMenu;
using UnityEngine;
using UnityEngine.UI;



namespace Quackery.Decks
{

    public class CardGameApp : App
    {
        private static readonly WaitForSeconds _waitForSeconds1 = new(1f);

        [Header("References")]

        [SerializeField] private ClientGameUI _clientGameUI;
        //  [SerializeField] private CartGauge _cartValue;
        [SerializeField] private BudgetCartValueUI _budgetCartValue;

        [SerializeField] private int _initialNumOfDraw;
        [SerializeField] private int _initialNumOfDiscard;

        //TODO:: Shouldn't be a CardPilePool, I think it is the Effect Pile Controller
        // To be confirmed
        [SerializeField] private CardPilePool _cardSelectPanel;
        [SerializeReference] private Button _EndRoundButton;
        [SerializeReference] private CounterButton _drawButton;
        [SerializeReference] private CounterButton _discardButton;
        [SerializeField] private EndOfRoundScreen _endRoundScreen;

        private Client _client;
        public bool RoundInterrupted { get; private set; }

        private bool _endButtonPressed;

        public static Action InterruptRoundRequest = delegate { };
        public static Action EndTransactionRequest = delegate { };
        private bool _endOfRound;

        void Awake()
        {
            //_cartValue.Hide();
            // _budgetCartValue.Hide();
            _EndRoundButton.interactable = false;
        }

        void OnEnable()
        {

            DeckEvents.OnCardsMovingToSelectPile += OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected += OnCardSelected;

            InterruptRoundRequest = () => RoundInterrupted = true;
            EndTransactionRequest = () => _endButtonPressed = true;

            _EndRoundButton.onClick.AddListener(() => _endButtonPressed = true);
            _discardButton.OnClicked += DiscardHand;
            _drawButton.OnClicked += DrawBackToFull;


        }


        void OnDisable()
        {

            DeckEvents.OnCardsMovingToSelectPile -= OnCardsMovingToSelectPile;
            DeckEvents.OnCardSelected -= OnCardSelected;

            InterruptRoundRequest = delegate { };
            EndTransactionRequest = delegate { };
            _EndRoundButton.onClick.RemoveAllListeners();
            _drawButton.OnClicked -= DrawBackToFull;
            _discardButton.OnClicked -= DiscardHand;
            StopAllCoroutines();

        }

        public void Reset()
        {
            CartServices.ResetCart();
            //  _cartValue.HideValue();
            _clientGameUI.Hide(instant: true);
            _EndRoundButton.gameObject.SetActive(false);
            _endRoundScreen.Hide(instant: true);
            _drawButton.gameObject.SetActive(false);
            _discardButton.gameObject.SetActive(false);


        }


        private void DrawBackToFull()
        {
            StartCoroutine(DrawBackToFullRoutine());

        }

        private IEnumerator DrawBackToFullRoutine()
        {
            SetButtonsInteractable(false);
            DeckServices.DeactivateHand();
            yield return DeckServices.DrawBackToFull();
            DeckServices.ActivateHand();
            SetButtonsInteractable(true);
        }

        private void DiscardHand()
        {
            StartCoroutine(DiscardHandRoutine());
        }

        private IEnumerator DiscardHandRoutine()
        {
            SetButtonsInteractable(false);
            DeckServices.DeactivateHand();
            yield return DeckServices.DiscardHand();
            // yield return DeckServices.DrawBackToFull();
            DeckServices.ActivateHand();
            SetButtonsInteractable(true);
        }

        private void OnCardSelected(Card card, List<Card> list)
        {
            //_cardSelectPanel.Hide();
        }

        private void OnCardsMovingToSelectPile()
        {
            //_cardSelectPanel.Show();
        }

        public void TransfertCartToPurse()
        {
            CartServices.ValidateCart();
            var cashInCart = CartServices.GetValue() + CartServices.GetBonus();

            if (cashInCart <= 0) return;

            PurseServices.Modify(cashInCart);
            CartServices.ResetCart();
        }

        internal void StartNewRound(Client client)
        {
            _EndRoundButton.interactable = false;
            RoundInterrupted = false;
            _endButtonPressed = false;
            _endOfRound = false;
            _drawButton.gameObject.SetActive(true);
            _discardButton.gameObject.SetActive(true);
            _client = client;
            StartCoroutine(RoundRoutine());

        }

        private IEnumerator RoundRoutine()
        {
            _drawButton.Counter = _initialNumOfDraw;
            _discardButton.Counter = _initialNumOfDiscard;
            yield return Tempo.WaitForABeat;
            yield return StartCoroutine(_clientGameUI.Show(_client));
            if (_client.IsAnonymous)
            {
                DialogQueueServices.QueueDialog("MeIntro");
                yield return DialogQueueServices.WaitUntilAllDialogEnds();
            }
            yield return _waitForSeconds1;
            yield return EffectServices.Add(_client);

            yield return Tempo.WaitForABeat;

            // if (_client.Budget > 0)
            //     _budgetCartValue.Show();
            // else
            //     _cartValue.Show();
            CartServices.InitCart();

            yield return _waitForSeconds1;
            yield return EffectServices.Execute(Effects.EnumEffectTrigger.OnRoundStart, null);
            bool firstHand = true;
            yield return DeckServices.DrawBackToFull();

            while (true)
            {
                if (!firstHand)
                    yield return EffectServices.UpdateDurationEffects();
                firstHand = false;

                DeckServices.ActivateHand();

                SetButtonsInteractable(true);


                yield return new WaitUntil(() => DeckServices.CardPlayed() ||
                                                 _endButtonPressed ||
                                                 RoundInterrupted);
                SetButtonsInteractable(false);


                if (RoundInterrupted)
                {
                    _endOfRound = true;
                    yield break;
                }

                if (_endButtonPressed == true)
                    break;

            }

            EffectServices.Execute(Effects.EnumEffectTrigger.OnRoundEnd, null);
            _endOfRound = true;
        }

        private void SetButtonsInteractable(bool isOn)
        {
            _drawButton.Interactable = isOn && !DeckServices.IsHandFull();
            _discardButton.Interactable = isOn && !DeckServices.IsHandEmpty();

            _EndRoundButton.gameObject.SetActive(true);
            _EndRoundButton.interactable = isOn;
            if (isOn && DeckServices.NoPlayableCards())
            {
                _EndRoundButton.transform.localScale = Vector3.one * 2f;
                _EndRoundButton.GetComponent<Image>().color = Color.green;
            }
            else
            {
                _EndRoundButton.transform.localScale = Vector3.one;
                _EndRoundButton.GetComponent<Image>().color = Color.white;
            }
        }


        internal WaitUntil WaitUntilEndOfRound()
        {
            return new WaitUntil(() => _endOfRound);
        }

        public void ShowEndRoundScreen(out bool wasBoss)
        {

            wasBoss = !ClientServices.IsCurrentClientAnonymous() &&
                      ClientServices.GetRevealedClient() == null;

            StartCoroutine(_endRoundScreen.Show(_client, !RoundInterrupted, CartServices.GetMode()));
        }
        public void HideEndRoundScreen()
        {
            _endRoundScreen.Hide(instant: false);
            _clientGameUI.Hide();
        }

        internal WaitUntil WaitUntilEndOfRoundScreenClosed()
        {
            float time = Time.time;

            return new WaitUntil(() =>
                Time.time - time > 10f ||
                !_endRoundScreen.gameObject.activeSelf);
        }

        public Coroutine StartTheWeek() => StartCoroutine(MondayRoutine());


        private IEnumerator MondayRoutine()
        {
            yield return new WaitForSeconds(1f);
            yield return GameMenuController.TakeDeckOut();
            yield return Tempo.WaitForABeat;
            yield return GameMenuController.TakeArtifactsOut();
        }
    }
}
