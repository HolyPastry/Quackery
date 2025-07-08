using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ChangePreference", menuName = "Quackery/Effects/ChangePreference")]
    public class ChangePreferenceEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        public EnumItemCategory Category => _category;

        public override void Execute(Effect effect)
        {
            EffectServices.ChangePreference(Category);
        }
    }
}
