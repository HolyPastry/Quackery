using Quackery.Decks;
using UnityEngine;


namespace Quackery
{
    public class CardPileTooltipExtension : TooltipExtension
    {


        public override void SetTooltip(GameObject hoveredObject)
        {
            if (!hoveredObject.TryGetComponent<CardPileUI>(out CardPileUI cardPileUI)) return;
            var card = cardPileUI.TopCard;
            if (card == null) return;

            string tooltip = FormatTooltip(card);

            _tooltipManager.AddTooltip(tooltip);

        }

        private string FormatTooltip(Card card)
        {
            string text = card.Item.Data.GetDescription();

            // foreach (var effect in card.Effects)
            // {
            //     text += effect.Description + "\n";
            // }


            return text;
        }

        // private void ShowNextToCard(Transform objectTransform)
        // {
        //     Canvas canvas = _tooltipManager.GetComponentInParent<Canvas>();

        //     transform.position = objectTransform.position + new Vector3((objectTransform as RectTransform).rect.width / 2 * canvas.scaleFactor, 0);
        // }


    }
}
