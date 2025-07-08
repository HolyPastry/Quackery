using System;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "TransmuteEffect", menuName = "Quackery/Effects/Card/TransmuteEffect", order = 1)]
    public class TransmuteEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Unset;
        public EnumItemCategory Category => _category;
        [SerializeField] private EnumCardSelection _cardSelection = EnumCardSelection.Random;

        public override void Execute(Effect effect)
        {
            DeckServices.ChangeCardCategory(Category, _cardSelection);
        }

        public override void Cancel(Effect effect)
        {
            DeckServices.RestoreCardCategories();
        }
    }
}
