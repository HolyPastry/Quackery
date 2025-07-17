using System.Collections;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RecountCart", menuName = "Quackery/Effects/Price/RecountCart", order = 0)]
    public class RecountCartEffect : EffectData
    {
        public enum Scope
        {
            Cart,
            Pile
        }
        [SerializeField] private Scope _scope;
        public override IEnumerator Execute(Effect effect)
        {
            yield return CartServices.CalculateCart();
        }
    }
}
