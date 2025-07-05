using System;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "TransmuteEffect", menuName = "Quackery/Effects/TransmuteEffect", order = 1)]
    public class TransmuteEffect : CategoryEffectData
    {
        // [SerializeField] private EnumItemCategory _newCategory;
        [SerializeField] private EnumCardSelection _cardSelection = EnumCardSelection.Random;

        // public override string GetDescription()
        // {
        //     return Sprites.Replace(_newCategory, Description);
        // }

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
