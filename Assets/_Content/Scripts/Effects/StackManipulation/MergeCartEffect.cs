using System;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "MergeCart", menuName = "Quackery/Effects/MergeCart", order = 0)]
    public class MergeCartEffect : CategoryEffectData
    {

        public override void Execute(Effect effect)
        {
            CartServices.MergeCart(effect.Value, Category);
        }
    }
}
