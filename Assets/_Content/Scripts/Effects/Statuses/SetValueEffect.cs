using System.Collections;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "SetValueEffect", menuName = "Quackery/Effects/Status/Set Effect Value", order = 0)]
    public class SetValueEffect : EffectData
    {
        [SerializeField] private EffectData _effectToModify;
        public override IEnumerator Execute(Effect effect)
        {
            EffectServices.SetValue(_effectToModify, effect.Value);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
