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
        public static Func<List<CardPile>> GetTablePile = () => new();

        public static Action<EnumCardPile, int> SelectCard = (pileType, index) => { };

        public static Action<EnumCardPile, EnumCardPile> MovePileType = (sourcePile, targetPile) => { };
        public static Action<CardPile, CardPile> MoveToPile = (source, target) => { };
        public static Func<EnumCardPile, Card> GetTopCard = (pileType) => null;

        public static Action<List<Card>> MoveToCardSelect = (cards) => { };
        public static Action InterruptDraw = delegate { };

        public static Action ResumeDraw = delegate { };
        public static Action<Card> MoveToTable = delegate { };
        public static Func<IEnumerator> DrawBackToFull = () => null;
        public static Func<int, List<Card>> Draw = (numberCards) => new();

        public static Action<Card> DestroyCard = (cards) => { };

        public static Action<List<ItemData>> DrawSpecificCards = (cards) => { };

        public static Action<EnumItemCategory, EnumCardSelection> ChangeCardCategory = delegate { };

        public static Action<int> DiscardCards = (amount) => { };

        public static Action RestoreCardCategories = () => { };


        public static Func<EnumItemCategory, Card> DrawCategory = (category) => null;

        public static Action ActivateTableCards = delegate { };

        public static Func<Card, Card> DuplicateCard = (card) => null;

        public static Func<bool> CardPlayed = () => false;
        public static Func<EnumCardPile, int> GetCardPoolSize = (cardPileType) => 0;

        public static Func<bool> NoPlayableCards = () => false;

        public static Func<ItemData, Card> CreateCard = (itemData) => null;

        public static Action<Card> StartPlayCardLoop = delegate { };

        public static Action StopPlayCardLoop = delegate { };

        public static Action<int, Predicate<Card>> BoostPriceOfCardsInHand = (value, predicate) => { };

        public static Func<ItemData, EnumCardPile, EnumPlacement, EnumLifetime, Card>
                                AddNew = (itemData, pileType, pileLocation, lifetime) => null;

        public static Action<Card, EnumCardPile, EnumPlacement, float> MoveCard = (card, pileType, placement, delay) => { };
        public static Action<Card, ItemData> ReplaceCard = (card, replacementCard) => { };

        internal static Func<Predicate<Card>, EnumCardPile, List<Card>> GetMatchingCards = (condition, pile) => new List<Card>();

        internal static Action ResetDecks = () => { };

    }
}
