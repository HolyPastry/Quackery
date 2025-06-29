using System;

namespace Quackery.Decks
{
    public static class CartEvents
    {
        public static Action<int> OnCalculatingCartPile = delegate { };
        public static Action<int> OnCartValueChanged = delegate { };
    }
}
