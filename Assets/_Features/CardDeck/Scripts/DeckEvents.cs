using System;
using System.Collections.Generic;
namespace Quackery.Decks
{
    public static class DeckEvents
    {
        public static Action<Card, EnumPileType, bool> OnCardMovedTo = (card, pileType, placeOnTop) => { };
        public static Action<EnumPileType> OnPileMoved = (destinationPile) => { };
        public static Action<EnumPileType> OnPileDestroyed = (pileType) => { };

        public static Action<EnumPileType, List<Card>> OnShuffle = (pileType, cards) => { };

    }
}
