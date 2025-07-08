using System;
using System.Collections.Generic;

namespace Quackery.Decks
{
    public static class CartEvents
    {
        public static Action<int> OnCalculatingCartPile = delegate { };
        public static Action<int> OnCartValueChanged = delegate { };

        public static Action<List<CardPile>, int> OnCartValidated = delegate { };

        internal static Action<List<int>> OnStacksHighlighted = delegate { };

    }
}
