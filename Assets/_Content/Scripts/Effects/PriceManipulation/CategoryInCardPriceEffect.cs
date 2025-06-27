using System;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "CategoryInCartPriceEffect", menuName = "Quackery/Effects/Category In Cart Price")]
    internal class CategoryInCartPriceEffect : EffectData
    {
        [SerializeField] private EnumItemCategory _category;
        public EnumItemCategory Category => _category;
    }
}
