using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ModifyCardSize", menuName = "Quackery/Effects/ModifyCartSize", order = 0)]
    public class ModifyCartSizeEffect : EffectData
    {
        public override void Execute(Effect effect)
        {
            CartServices.ModifyCardCartSizeModifier(effect.Value);
        }

        public override void Cancel(Effect effect)
        {
            CartServices.ModifyCardCartSizeModifier(-effect.Value);
        }
    }
}
