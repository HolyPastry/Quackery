using System.Collections.Generic;

using System;
using Quackery.Decks;
using UnityEngine;
using Quackery.Inventories;
using Quackery.Effects;
using System.Collections;



namespace Quackery
{

    public static class DeckServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);


        public static Action Shuffle = () => { };
        public static Action<List<Card>> Discard = (card) => { };
        public static Func<IEnumerator> DiscardHand = () => null;


        internal static Func<List<CardPile>> GetTablePile = () => new();

        internal static Action<EnumCardPile, int> PileClicked = (pileType, index) => { };


        internal static Action<EnumCardPile, EnumCardPile> MovePileType = (sourcePile, targetPile) => { };
        public static Action<CardPile, CardPile> MoveToPile = (source, target) => { };
        internal static Func<EnumCardPile, Card> GetTopCard = (pileType) => null;

        internal static Action<List<Card>> MoveToCardSelect = (cards) => { };
        internal static Action InterruptDraw = delegate { };

        internal static Action ResumeDraw = delegate { };
        internal static Action<Card> MoveToTable = delegate { };
        internal static Func<IEnumerator> DrawBackToFull = () => null;
        public static Func<int, List<Card>> Draw = (numberCards) => new();

        internal static Action<Card> DestroyCard = (cards) => { };



        internal static Action<List<ItemData>> DrawSpecificCards = (cards) => { };

        internal static Action<EnumItemCategory, EnumCardSelection> ChangeCardCategory = delegate { };

        internal static Action<int> DiscardCards = (amount) => { };

        internal static Action RestoreCardCategories = () => { };





        internal static Func<EnumItemCategory, Card> DrawCategory = (category) => null;

        public static Action ActivateTableCards = delegate { };

        internal static Func<Card, Card> DuplicateCard = (card) => null;

        internal static Action<ItemData, int> AddNewToDiscard = (card, amount) => { };

        // internal static Action<ItemData, int> AddMultipleInstancesToDrawDeck = (card, amount) => { };
        // public static Action<List<ItemData>> AddToDrawPile = (cards) => { };
        public static Action<ItemData, bool> AddNewToDraw = (card, isPermanent) => { };

        internal static Func<bool> CardPlayed = () => false;
        internal static Func<EnumCardPile, int> GetCardPoolSize = (cardPileType) => 0;

        public static Func<int> GetLastCartPileIndex = () => 0;

        internal static Func<bool> NoPlayableCards = () => false;

        internal static Func<ItemData, Card> CreateCard = (itemData) => null;
    }
}
