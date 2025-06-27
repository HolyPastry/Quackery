using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "StackMultiplier", menuName = "Quackery/Effects/StackMultiplier", order = 0)]
    public class StackMultiplierEffect : EffectData
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        public EnumItemCategory Category => _category;
    }
}
