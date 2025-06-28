using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Inventories;

namespace Quackery.Decks
{
    public static class CartServices
    {
        public static Action DiscardCart = delegate { };
        public static Action<int, EnumItemCategory> MergeCart = (amount, category) => { };
        public static Func<EnumItemCategory, int> GetNumCardInCart = (category) => 0;
        public static Func<List<EnumItemCategory>> GetCategoriesInCart = () => new();

        public static Action<int> SetRatingCartSizeModifier = (value) => { };
        public static Action<int> ModifyCardCartSizeModifier = (value) => { };
        public static Func<IEnumerator> CalculateCart = () => null;
        public static Action CompleteCartPileCalculation = () => { };

        public static Func<Card, bool> AddCardToCart = (pile) => false;

        public static Func<Card, bool> CanAddToCart = (card) => true;
        public static Func<int> GetCartSize = () => 0;

        public static Action RestoreCategories = () => { };

        public static Action<EnumItemCategory> ChangeCardCategory = (category) => { };

        public static Action<Card> RemoveCard = (card) => { };

        public static Func<EnumCardPile, int, List<CardReward>> GetPileRewards = (type, index) => new List<CardReward>();

        public static Action<int> AddToCartValue = (value) => { };

        public static Func<int> GetCartValue = () => 0;
        public static Action<int> SetCartValue = (value) => { };
        public static Func<int, bool> CanCartAfford = (value) => { return true; };

    }
}
