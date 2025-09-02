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
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => false);

        public static Func<ItemData, Card> CreateCard = (itemData) => null;
        internal static Func<ItemData, Coroutine> DestroyCardType = (itemData) => null;

        public static Action<Card> RemoveFromAllPiles = (card) => { };

        public static Action Shuffle = () => { };
        public static Func<List<Card>, Coroutine> Discard = (card) => null;
        public static Func<Coroutine> DiscardHand = () => null;
        public static Func<int, Coroutine> DiscardCards = (amount) => null;

        public static Func<List<CardPile>> GetTablePile = () => new();

        public static Action<EnumCardPile, int> SelectCard = (pileType, index) => { };

        public static Action<EnumCardPile, EnumCardPile> MovePileType = (sourcePile, targetPile) => { };
        public static Action<CardPile, CardPile> MoveToPile = (source, target) => { };
        public static Func<EnumCardPile, Card> GetTopCard = (pileType) => null;

        public static Action<List<Card>> MoveToCardSelect = (cards) => { };

        public static Action<Card> MoveToTable = delegate { };
        public static Func<Coroutine> DrawBackToFull = () => null;

        public static Func<Card, Coroutine> DestroyCard = (cards) => null;

        public static Action<List<ItemData>> ForceOnNextDraw = (cards) => { };

        public static Action<EnumItemCategory, EnumCardSelection> ChangeCardCategory = delegate { };



        public static Action RestoreCardCategories = () => { };

        public static Func<EnumItemCategory, Card> DrawCategory = (category) => null;

        public static Action ActivateTableCards = delegate { };

        public static Func<Card, Card> DuplicateCard = (card) => null;

        public static Func<bool> CardPlayed = () => false;
        public static Func<EnumCardPile, int> GetCardPoolSize = (cardPileType) => 0;

        public static Func<bool> NoPlayableCards = () => false;


        public static Action<Card> StartPlayCardLoop = delegate { };

        public static Action StopPlayCardLoop = delegate { };

        public static Action<int, Predicate<Card>> BoostPriceOfCardsInHand = (value, predicate) => { };

        public static Func<ItemData, EnumCardPile, EnumPlacement, EnumLifetime, Card>
                                AddNew = (itemData, pileType, pileLocation, lifetime) => null;

        public static Func<Card, EnumCardPile, EnumPlacement, float, Coroutine> MoveCard = (card, pileType, placement, delay) => null;

        internal static Func<Predicate<Card>, EnumCardPile, List<Card>> GetMatchingCards = (condition, pile) => new List<Card>();

        internal static Action ResetDecks = () => { };

        internal static Action<int> SetCustomDraw = (numCard) => { };


        internal static Func<EnumCardPile, int, bool> IsPilePlayable = (pile, index) => true;

        internal static Action DestroyEffemeralCards = () => { };

    }
}
