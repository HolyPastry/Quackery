using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.GameStats
{

    public enum EnumPriceCondition
    {
        Any,
        OriginalPrice,
        ReducedPrice,
        Overpriced
    }

    [Serializable]
    public struct CardStats
    {
        [HideInInspector]
        public string Key;
        public EnumPriceCondition PriceCondition;
        public EnumItemCategory Category;

        [HideInInspector]
        public int BasePrice;
        [HideInInspector]
        public int SoldPrice;

        public CardStats(Card card)
        {
            Key = card.Item.Key;
            Category = card.Category;
            SoldPrice = card.Price;
            BasePrice = card.Item.BasePrice;
            PriceCondition = EnumPriceCondition.Any;
            if (SoldPrice == BasePrice)
                PriceCondition = EnumPriceCondition.OriginalPrice;
            if (SoldPrice > BasePrice)
                PriceCondition = EnumPriceCondition.Overpriced;
            if (SoldPrice < BasePrice)
                PriceCondition = EnumPriceCondition.ReducedPrice;
        }
    }

    [Serializable]
    public struct CartStats
    {
        public int NumCards;
        public int TotalPrice;

    }

    public class SerialGameStats : SerialData
    {
        public List<CardStats> CardStats = new();
        public List<CartStats> CartStats = new();


    }
    public class GameStatsManager : Service
    {

        [SerializeField] private SerialGameStats _gameStats;

        void OnEnable()
        {
            GameStatsServices.WaitUntilReady = () => WaitUntilReady;
            GameStatsServices.NumMatchingCard = NumMatchingCards;

            DeckEvents.OnCardPlayed += OnCardPlayed;
            CartEvents.OnCartValidated += OnCartValidated;
        }


        void OnDisable()
        {
            GameStatsServices.WaitUntilReady = () => new WaitUntil(() => true);
            GameStatsServices.NumMatchingCard = (stats) => 0;

            DeckEvents.OnCardPlayed -= OnCardPlayed;
            CartEvents.OnCartValidated -= OnCartValidated;
        }
        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();

            _gameStats = SaveServices.Load<SerialGameStats>("GameStats");
            _gameStats ??= new SerialGameStats();
            _isReady = true;
        }

        private void Save()
        {
            SaveServices.Save("GameStats", _gameStats);
        }

        private int NumMatchingCards(CardStats conditions)
        {
            return _gameStats.CardStats.Count(card =>

                MatchPriceCondition(card, conditions) &&
                MatchCategoryCondition(card, conditions)
            );
        }

        private bool MatchCategoryCondition(CardStats card, CardStats conditions)
        {
            if (conditions.Category == EnumItemCategory.Any) return true;
            return card.Category == conditions.Category;
        }

        private bool MatchPriceCondition(CardStats card, CardStats conditions)
        {
            if (conditions.PriceCondition == EnumPriceCondition.Any) return true;
            return card.PriceCondition == conditions.PriceCondition;
        }

        private void OnCardPlayed(Card card)
        {
            CardStats stats = new(card);
            _gameStats.CardStats.Add(stats);
            Save();
        }

        private void OnCartValidated(List<CardPile> cart, int amount)
        {
            CartStats stats = new()
            {
                NumCards = cart.SelectMany(pile => pile.Cards).Count(),
                TotalPrice = amount
            };
            _gameStats.CartStats.Add(stats);
            Save();
        }


    }
}
