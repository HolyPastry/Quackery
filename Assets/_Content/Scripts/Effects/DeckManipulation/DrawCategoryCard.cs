using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "DrawCategoryCard", menuName = "Quackery/Effects/Draw Category Card", order = 0)]
    public class DrawCategoryCardEffect : CategoryEffectData
    {

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
