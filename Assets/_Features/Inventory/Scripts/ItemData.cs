

using System;
using System.Collections.Generic;
using System.Linq;
using Holypastry.Bakery;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Inventories
{

    [CreateAssetMenu(
        fileName = "ItemData",
        menuName = "Quackery/ItemData",
        order = 1)]
    public class ItemData : ContentTag
    {
        public Sprite Icon;

        public int BasePrice = 1;

        [TextArea(2, 10)]
        public string ShortDescription;

        [TextArea(3, 10)]
        public string LongDescription;

        [TextArea(3, 10)]
        public string ShopDescription;

        public List<Explanation> Explanations = new();

        public int ShopPrice;

        public EnumItemCategory Category;

        public List<EffectData> Effects = new();

        public EnumRarity Rarity;

        internal void CheckValidity()
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("ItemData must have a name.");
            }
            if (Icon == null)
            {
                Debug.LogWarning($"ItemData {name} must have an icon.");
            }
            Effects.ForEach(effect =>
            {
                if (effect == null)
                {
                    Debug.LogWarning($"ItemData {name} has a null effect.");
                }
                else
                {
                    effect.CheckValidity();
                }
            });
        }
    }
}