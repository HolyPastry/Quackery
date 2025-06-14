using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Quackery
{
    public class EffectTooltipExtension : TooltipExtension
    {
        public override void SetTooltip(GameObject hoveredObject)
        {
            if (hoveredObject.TryGetComponent(out EffectUI effectUI))
                AddEffectTooltip(effectUI);

            if (hoveredObject.TryGetComponent(out StackMultiplierUI stackMultiplierUI))
                _tooltipManager.AddTooltip($"Stack Multiplier: x{stackMultiplierUI.MultiplierText}");

            ShowNextToStatus(hoveredObject.transform);
        }

        private void AddEffectTooltip(EffectUI effectUI)
        {

            _tooltipManager.AddTooltip(effectUI.Effect.Description);

            _tooltipManager.RefreshLayout();

        }

        private void ShowNextToStatus(Transform objectTransform)
        {
            Canvas canvas = _tooltipManager.GetComponentInParent<Canvas>();

            transform.position = objectTransform.position + new Vector3((objectTransform as RectTransform).rect.width / 2 * canvas.scaleFactor, 0);
        }
    }
}
