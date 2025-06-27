using System.Collections.Generic;

using System;
using Quackery.Decks;
using UnityEngine;
using Quackery.Inventories;
using Quackery.Effects;



namespace Quackery
{

    public static class DeckServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);


        public static Action Shuffle = () => { };
        public static Action<List<Card>> Discard = (card) => { };
        public static Action DiscardHand = () => { };
        public static Action<List<ItemData>> AddToDrawPile = (cards) => { };


        internal static Func<List<CardPile>> GetTablePile = () => new();

        internal static Action<EnumPileType> PileClicked = (pileType) => { };

        internal static Action ShuffleDiscardIn = () => { };
        internal static Func<EnumPileType, List<CardReward>> GetPileRewards = (pileType) => new();
        internal static Func<EnumPileType, int> EvaluatePileValue = (pileType) => 0;

        public static Func<bool> IsCartFull = () => false;

        internal static Action<EnumPileType, EnumPileType> MovePileTo = (sourcePile, targetPile) => { };
        internal static Func<EnumPileType, Card> GetTopCard = (pileType) => null;


        internal static Action DiscardCart = delegate { };
        internal static Action<List<Card>> MoveToCardSelect = (cards) => { };
        internal static Action InterruptDraw = delegate { };
        internal static Action<Card> MoveToTable = delegate { };

        internal static Action DrawBackToFull = delegate { };
        public static Func<int, List<Card>> Draw = (numberCards) => new();

        internal static Action<Card> DestroyCard = (cards) => { };

        internal static Action<int> SetCartSize = (newSize) => { };

        internal static Action<int> ModifyCartSize = (amount) => { };

        internal static Action<int, EnumItemCategory> MergeCart = (amount, category) => { };

        internal static Action RecountCart = () => { };

        internal static Action<ItemData> RemoveCard = (card) => { };

        internal static Action RemoveCardRequest = delegate { };
        internal static Action<List<ItemData>> DrawSpecificCards = (cards) => { };

        public static Func<WaitUntil> WaitUntilCardStopMoving = () => new WaitUntil(() => true);

        internal static Action<EnumItemCategory, EnumCardSelection> ChangeCardCategory = delegate { };

        internal static Action<int> DiscardCards = (amount) => { };

        internal static Action RestoreCardCategories = () => { };

        internal static Func<EnumItemCategory, int> GetNumCardInCart = (category) => 0;

        internal static Action<CardPile> RecountPile = (pile) => { };

        internal static Action<CardPile> ReplaceTopCard = (pile) => { };

        internal static Func<EnumItemCategory, Card> DrawCategory = (category) => null;

        public static Action ActivateTableCards = delegate { };

        internal static Func<List<EnumItemCategory>> GetCategoriesInCard = () => new();

        internal static Func<Card, Card> DuplicateCard = (card) => null;

        internal static Action<ItemData, int> AddNewToDiscard = (card, amount) => { };

        internal static Action<ItemData, int> AddNewToDrawDeck = (card, amount) => { };
    }
}
