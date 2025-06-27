using System;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "MergeCart", menuName = "Quackery/Effects/MergeCart", order = 0)]
    public class MergeCartEffect : EffectData
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        public override void Execute(Effect effect, CardPile pile)
        {
            DeckServices.MergeCart(effect.Value, _category);
        }
    }
}
