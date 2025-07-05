using Quackery.Decks;
using UnityEditor;
using UnityEngine;


namespace Quackery
{
    public class CardPileTooltipExtension : TooltipExtension
    {


        public override void SetTooltip(GameObject hoveredObject)
        {
            if (!hoveredObject.TryGetComponent<CardPileUI>(out CardPileUI cardPileUI)) return;
            if (cardPileUI.IsEmpty) return;
            var card = cardPileUI.TopCard;
            if (card == null) return;

            _tooltipManager.AddTooltip(card.Name, card.Item.Data.LongDescription, card.Item.Data.Explanations);

        }
    }
}
