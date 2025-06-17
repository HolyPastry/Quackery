using System;
using Quackery.Decks;

using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class CardPileTooltipExtension : TooltipExtension
    {


        public override void SetTooltip(GameObject hoveredObject)
        {
            if (!hoveredObject.TryGetComponent<CardPileUI>(out CardPileUI cardPileUI)) return;
            var card = DeckServices.GetTopCard(cardPileUI.Type);
            if (card == null) return;

            _tooltipManager.AddTooltip($"Stack: {cardPileUI.StackSize}, Value: {DeckServices.EvaluatePileValue(cardPileUI.Type)}");


            foreach (var power in card.Effects)
                _tooltipManager.AddTooltip(power.Description);

            _tooltipManager.RefreshLayout();
            ShowNextToCard(hoveredObject.transform);

        }

        private void ShowNextToCard(Transform objectTransform)
        {
            Canvas canvas = _tooltipManager.GetComponentInParent<Canvas>();

            transform.position = objectTransform.position + new Vector3((objectTransform as RectTransform).rect.width / 2 * canvas.scaleFactor, 0);
        }


    }
}
