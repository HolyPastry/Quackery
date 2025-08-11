using System;
using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "TransmuteEffect", menuName = "Quackery/Effects/Card/TransmuteEffect", order = 1)]
    public class TransmuteEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;
        [SerializeField] private EnumCardSelection _cardSelection = EnumCardSelection.Random;

        public override IEnumerator Execute(Effect effect)
        {
            DeckServices.ChangeCardCategory(Category, _cardSelection);
            yield return DefaultWaitTime;
        }

        public override void OnRemove(Effect effect)
        {
            DeckServices.RestoreCardCategories();
        }
    }
}
