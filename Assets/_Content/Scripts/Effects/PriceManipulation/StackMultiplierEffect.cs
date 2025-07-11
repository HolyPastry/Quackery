using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "StackMultiplier", menuName = "Quackery/Effects/Price/StackMultiplier", order = 0)]
    public class StackMultiplierEffect : EffectData, ICategoryEffect, IOperationEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        [SerializeField] private EnumOperation _operation = EnumOperation.Multiply;

        public EnumItemCategory Category => _category;

        public EnumOperation Operation => _operation;
    }
}
