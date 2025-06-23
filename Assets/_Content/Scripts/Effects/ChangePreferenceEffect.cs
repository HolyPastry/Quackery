using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ChangePreference", menuName = "Quackery/Effects/ChangePreference")]
    public class ChangePreferenceEffect : EffectData
    {
        public EnumItemCategory Category;
        public override void Execute(Effect effect, CardPile pile)
        {
            EffectServices.ChangePreference(Category);
        }
    }
}
