using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Decks
{
    public static class CartServices
    {
        public static Action DiscardCart = delegate { };
        public static Action<int, EnumItemCategory> MergeCart = (amount, category) => { };
        public static Func<EnumItemCategory, int> GetNumCardInCart = (category) => 0;
        public static Func<List<EnumItemCategory>> GetCategoriesInCart = () => new();

        public static Action<int> SetRatingCartSizeModifier = (value) => { };
        // public static Action<int> ModifyCardCartSizeModifier = (value) => { };
        public static Func<Coroutine> CalculateCart = () => null;

        public static Func<Card, bool> AddCardToCart = (pile) => false;

        public static Func<Card, bool> CanAddToCart = (card) => true;
        public static Func<int> GetCartSize = () => 0;

        public static Action RestoreCategories = () => { };

        public static Action<EnumItemCategory> ChangeCardCategory = (category) => { };

        public static Action<Card> RemoveCard = (card) => { };

        public static Action<int> AddToCartValue = (value) => { };

        public static Func<int> GetCartValue = () => 0;
        public static Action<int> SetCartValue = (value) => { };
        public static Func<int, bool> CanCartAfford = (value) => { return true; };

        // internal static Action ResetCartSizeCardModifier = () => { };

        internal static Action<Card> ReplaceTopCard = (card) => { };

        internal static Func<Card> GetTopCard = () => null;

        internal static Func<int> GetLastCartValue = () => 0;

        internal static Action ValidateCart = () => { };

        internal static Func<Card, CardPile, bool> AddCardToCartPile = (card, pile) => false;

        internal static Action<Card> SetStacksHighlights = (card) => { };
        internal static Func<CardPile> GetHoveredPile = () => null;

        public static Action<int> HoverPile = (index) => { };

        public static Action<int> UnhoverPile = (index) => { };

        public static Func<int> GetCartBonus = () => 0;

        public static Action RandomizeCart = () => { };

        internal static Func<CartEvaluation> GetCartEvaluation = () => default;

    }
}
