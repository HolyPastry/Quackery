using System;
using System.Collections.Generic;
namespace Quackery.Decks
{
    public static class DeckEvents
    {
        public static Action<Card, EnumPileType, bool> OnCardMovedTo = (card, pileType, placeOnTop) => { };
        public static Action<EnumPileType> OnPileMoved = (destinationPile) => { };
        public static Action<EnumPileType> OnCashingTheCart = (destinationPile) => { };
        public static Action<EnumPileType> OnPileDestroyed = (pileType) => { };

        public static Action<EnumPileType, List<Card>> OnShuffle = (pileType, cards) => { };

        public static Action OnCardsMovingToSelectPile = () => { };

        public static Action<Card, List<Card>> OnCardSelected = (selectedCard, otherCards) => { };

        internal static Action<int> OnCartSizeUpdated = (size) => { };

        public static Action<EnumPileType, bool> OnActivatePile { get; internal set; }

        internal static Action<CardPile> OnCashingPile = (pile) => { };
        internal static Action<EnumPileType> OnPileUpdated = (pileType) => { };
    }
}
