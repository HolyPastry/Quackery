using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ModifyCardSize", menuName = "Quackery/Effects/Cart/ModifySize", order = 0)]
    public class CartSizeModifierEffect : EffectData, IStatusEffect
    {
        [SerializeField] private Status _status;
        [SerializeField] private int _sizeModifier;
        public Status Status => _status;

        public float Value => _sizeModifier;
    }
}
