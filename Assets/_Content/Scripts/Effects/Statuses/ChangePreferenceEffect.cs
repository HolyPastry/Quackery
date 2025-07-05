using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "ChangePreference", menuName = "Quackery/Effects/ChangePreference")]
    public class ChangePreferenceEffect : CategoryEffectData
    {


        public override void Execute(Effect effect)
        {
            EffectServices.ChangePreference(Category);
        }
    }
}
