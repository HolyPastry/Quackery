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


        public static Action Shuffle = () => { };
        public static Action<List<Card>> Discard = (card) => { };
        public static Action DiscardHand = () => { };
        public static Action<List<ItemData>> AddToDeck = (cards) => { };


        internal static Func<List<CardPile>> GetTablePile = () => new();

        internal static Action<EnumPileType> PileClicked = (pileType) => { };

        internal static Action ShuffleDiscardIn = () => { };
        internal static Func<EnumPileType, List<CardReward>> GetPileRewards = (pileType) => new();
        internal static Func<EnumPileType, int> EvaluatePileValue = (pileType) => 0;

        public static Func<bool> IsCartFull = () => false;

        internal static Action<CardPile, CardPile> MovePileTo = (sourcePile, targetPile) => { };
        internal static Func<EnumPileType, Card> GetTopCard = (pileType) => null;


        internal static Action DiscardCart = delegate { };
        internal static Action<List<Card>> MoveToCardSelect = (cards) => { };
        internal static Action InterruptDraw = delegate { };
        internal static Action<Card> MoveToTable = delegate { };

        internal static Action DrawBackToFull = delegate { };
        public static Func<int, List<Card>> Draw = (numberCards) => new();

        internal static Action<Card> Destroy = (cards) => { };

        internal static Action<int> SetCartSize = (newSize) => { };

        internal static Action<int> ExpandCart = (amount) => { };

        internal static Action<int> MergeCart = (amount) => { };

        internal static Action RecountCart = () => { };

        internal static Action<ItemData> RemoveCard = (card) => { };

        internal static Action RemoveCardRequest = delegate { };
        internal static Action<List<ItemData>> DrawSpecificCards = (cards) => { };


        internal static Action<EnumItemCategory> ChangeRandomgTableCardCategory = (category) => { };

        internal static Action<int> DiscardCards = (amount) => { };

    }
}
