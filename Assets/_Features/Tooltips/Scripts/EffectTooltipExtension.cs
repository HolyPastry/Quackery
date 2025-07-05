using System;

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
            //string description = Sprites.ReplaceCategory(effect.Description);
            //string title = effect.Data.MasterText;



            _tooltipManager.AddTooltip(effect.Data.MasterText, effect.Description, effect.Data.Explanations);
        }

        // private void ShowNextToStatus(Transform objectTransform)
        // {
        //     Canvas canvas = _tooltipManager.GetComponentInParent<Canvas>();

        //     transform.position = objectTransform.position + new Vector3((objectTransform as RectTransform).rect.width / 2 * canvas.scaleFactor, 0);
        // }
    }
}
