using System;
using System.Collections.Generic;

namespace Quackery.Decks
{
    public static class CartEvents
    {
        public static Action OnValueChanged = delegate { };
        public static Action<List<CardPile>, int> OnCartValidated = delegate { };
        public static Action OnCartCleared = delegate { };
        internal static Action<List<CardPile>> OnStacksHighlighted = delegate { };
        public static Action<Card> OnNewCartPileUsed = delegate { };
        public static Action<Card, CardPile> OnStackHovered = delegate { };
        internal static Action<int, CardReward, int, float> OnCartRewardCalculated = (pileIndex, reward, deltaScore, duration) => { };
        public static Action<CartMode> OnModeChanged = delegate { };

        internal static Action<int> OnBonusChanged = (deltaScore) => { };
    }
}
