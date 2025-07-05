using System;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    public class EffectTooltipExtension : TooltipExtension
    {
        public override void SetTooltip(GameObject hoveredObject)
        {
            if (hoveredObject.TryGetComponent(out EffectUI effectUI))
                AddEffectTooltip(effectUI.Effect);

            // if (hoveredObject.TryGetComponent(out StackMultiplierUI stackMultiplierUI))
            //     _tooltipManager.AddTooltip($"Stack Multiplier: x{stackMultiplierUI.MultiplierText}");

            // ShowNextToStatus(hoveredObject.transform);
        }

        private void AddEffectTooltip(Effect effect)
        {
            string description = effect.Description;
            string title = effect.Data.MasterText;
            if (effect.Data is CategoryEffectData categoryEffectData)
            {
                description = Sprites.ReplaceCategory(description, categoryEffectData.Category);
                title = Sprites.ReplaceCategory(title, categoryEffectData.Category);
            }


            _tooltipManager.AddTooltip(title, description, effect.Data.Explanations);
        }

        // private void ShowNextToStatus(Transform objectTransform)
        // {
        //     Canvas canvas = _tooltipManager.GetComponentInParent<Canvas>();

        //     transform.position = objectTransform.position + new Vector3((objectTransform as RectTransform).rect.width / 2 * canvas.scaleFactor, 0);
        // }
    }
}
