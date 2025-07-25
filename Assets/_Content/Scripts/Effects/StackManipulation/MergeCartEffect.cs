using System;
using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "MergeCart", menuName = "Quackery/Effects/Stack/MergeCart", order = 0)]
    public class MergeCartEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;
        public override IEnumerator Execute(Effect effect)
        {
            CartServices.MergeCart(effect.Value, Category);
            yield return DefaultWaitTime;
        }
    }
}
