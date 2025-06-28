using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ModifyEffectValue", menuName = "Quackery/Effects/Modify Effect Value", order = 1)]
    public class ModifyEffectValue : EffectData
    {
        [SerializeField] private EffectData _effectToModify;

        public override void Cancel(Effect effect)
        {
            EffectServices.ModifyValue(_effectToModify, -effect.Value);
        }

        public override void Execute(Effect effect)
        {
            EffectServices.ModifyValue(_effectToModify, effect.Value);
        }
    }
}
