using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "EffectAmplifierEffect", menuName = "Quackery/Effects/Status/Effect Amplifier")]
    public class EffectAmplifierEffect : EffectData
    {
        [SerializeField] private EffectData _effectToAmplify;
        public EffectData EffectToAmplify => _effectToAmplify;

    }
}
