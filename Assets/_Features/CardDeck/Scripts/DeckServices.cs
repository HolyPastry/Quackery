using System.Collections.Generic;

using System;
using Quackery.Decks;
using UnityEngine;
using Quackery.Inventories;



namespace Quackery
{

    public static class DeckServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
        public static Func<Card> DrawOne = () => null;
        public static Func<int, List<Card>> DrawMany = (numberCards) => new();
        public static Action Shuffle = () => { };
        public static Action<Card> Discard = (card) => { };
        public static Action DiscardHand = () => { };
        public static Action<List<Card>> AddToCart = (card) => { };
        public static Action<List<ItemData>> AddToDeck = (cards) => { };
        public static Action<EnumPileType> DestroyPile = (pileType) => { };
        public static Action ResetDeck = () => { };
        internal static Action<Card> DestroyCard = (card) => { };
        internal static Func<List<CardPile>> GetTablePile = () => new();
        internal static Action<Card, Card> MovetoCardInCart = (card, targetCard) => { };
        internal static Action<EnumPileType> PileClicked = (pileType) => { };
        internal static Func<int> GetTotalCashInCart = () => 0;
        internal static Action ShuffleDiscardIn = () => { };
        internal static Func<EnumPileType, List<CardReward>> GetPileRewards = (pileType) => new();
        public static Func<bool> IsCartFull = () => false;
        public static Func<EnumPileType, CardPile> GetPile;
        internal static Action<CardPile, CardPile> MovePileTo = (sourcePile, targetPile) => { };
        internal static Func<EnumPileType, Card> GetTopCard = (pileType) => null;
        internal static Func<EnumPileType, int> EvaluatePileValue = (pileType) => 0;
    }
}
