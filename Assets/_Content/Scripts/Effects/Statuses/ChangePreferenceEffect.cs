using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ChangePreference", menuName = "Quackery/Effects/ChangePreference")]
    public class ChangePreferenceEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;

        public override IEnumerator Execute(Effect effect)
        {
            EffectServices.ChangePreference(Category);
            yield return DefaultWaitTime;
        }
    }
}
