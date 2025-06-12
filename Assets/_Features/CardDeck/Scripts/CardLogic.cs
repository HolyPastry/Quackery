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

        private int _cashInCart = 0;


        void OnEnable()
        {

            _EndTheDayButton.onClick.AddListener(EndTheDay);
            _StartTheDayButton.onClick.AddListener(() => StartRound());

            DeckEvents.OnPileMoved += OnPileMoved;
        }



        void OnDisable()
        {
            _EndTheDayButton.onClick.RemoveListener(EndTheDay);
            _StartTheDayButton.onClick.RemoveAllListeners();

            DeckEvents.OnPileMoved -= OnPileMoved;
        }

        private void OnPileMoved(EnumPileType type)
        {
            if (type != EnumPileType.InCart1 && type != EnumPileType.InCart2 && type != EnumPileType.InCart3)
                return;


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
                Debug.Log($"Card Reward: {cardReward.Type}, Price: {cardReward.Value}");
            }

            _CashInCartText.text = $"Cash in Cart: {_cashInCart}";


            yield return new WaitForSeconds(0.5f);
            if (DeckServices.IsCartFull())
            {
                StartCoroutine(EndOfRoundRoutine());
            }
            else
            {
                DeckServices.DrawOne();
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

            DeckServices.DestroyPile(EnumPileType.InCart1);
            DeckServices.DestroyPile(EnumPileType.InCart2);
            DeckServices.DestroyPile(EnumPileType.InCart3);

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

            DeckServices.Shuffle();
            yield return new WaitForSeconds(2f);
            DeckServices.DrawMany(4);
        }
    }

}
