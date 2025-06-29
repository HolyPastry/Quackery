using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RecountCart", menuName = "Quackery/Effects/RecountCart", order = 0)]
    public class RecountCartEffect : EffectData
    {
        public enum Scope
        {
            Cart,
            Pile
        }
        [SerializeField] private Scope _scope;
        public override void Execute(Effect effect)
        {
            effect.LinkedCard.StartCoroutine(CartServices.CalculateCart());
        }
    }
}
