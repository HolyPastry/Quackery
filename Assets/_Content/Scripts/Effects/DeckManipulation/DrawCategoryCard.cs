using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "DrawCategoryCard", menuName = "Quackery/Effects/Deck/Draw Category Card", order = 0)]
    public class DrawCategoryCardEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;
        public override void Execute(Effect effect)
        {
            DeckServices.InterruptDraw();
            Card drawnCard = DeckServices.DrawCategory(Category);
            if (drawnCard != null)
                DeckServices.MoveToTable(drawnCard);
            //DeckServices.DrawBackToFull();
        }
    }
}
