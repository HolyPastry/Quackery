using System;
using System.Collections.Generic;
namespace Quackery.Decks
{
    public static class DeckEvents
    {
        public static Action<Card, EnumCardPile, int, bool, bool> OnCardMovedTo = (card, pileType, index, placeOnTop, isInstant) => { };
        public static Action<EnumCardPile, int> OnPileDestroyed = (pileType, index) => { };

        public static Action<EnumCardPile, int, List<Card>> OnShuffle = (pileType, index, cards) => { };

        public static Action OnCardsMovingToSelectPile = () => { };

        public static Action<Card, List<Card>> OnCardSelected = (selectedCard, otherCards) => { };

        internal static Action<EnumCardPile> OnCardPoolSizeUpdate = (cardPile) => { };

        public static Action<EnumCardPile, int, bool> OnActivatePile = (pileType, index, activate) => { };

        internal static Action<EnumCardPile, int> OnPileUpdated = (pileType, index) => { };

        public static Action<Card> OnCardPlayed { get; internal set; }

    }
}
