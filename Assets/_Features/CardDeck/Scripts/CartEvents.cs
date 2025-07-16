using System;
using System.Collections.Generic;

namespace Quackery.Decks
{
    public static class CartEvents
    {
        public static Action OnCartValueChanged = delegate { };
        public static Action<List<CardPile>, int> OnCartValidated = delegate { };
        public static Action OnCartCleared = delegate { };
        internal static Action<List<int>> OnStacksHighlighted = delegate { };
        public static Action<Card> OnNewCartPileUsed = delegate { };
        public static Action<Card, CardPile> OnStackHovered = delegate { };

        internal static Action<int, CardReward, float> OnCartRewardCalculated = (pileIndex, reward, duration) => { };
    }
}
