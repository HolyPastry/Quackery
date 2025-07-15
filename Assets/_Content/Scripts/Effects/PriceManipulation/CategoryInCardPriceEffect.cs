using System;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "CategoryInCartPriceEffect", menuName = "Quackery/Effects/Price/Category In Cart Price")]
    internal class CategoryInCartPriceEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;
    }
}
