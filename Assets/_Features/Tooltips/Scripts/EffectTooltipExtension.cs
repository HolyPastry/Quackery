using UnityEngine;

namespace Quackery
{
    public class EffectTooltipExtension : TooltipExtension
    {
        public override void SetTooltip(GameObject hoveredObject)
        {
            if (!hoveredObject.TryGetComponent(out EffectUI effectUI)) return;

            _tooltipManager.AddTooltip(effectUI.Effect.Description);

            _tooltipManager.RefreshLayout();
            ShowNextToStatus(hoveredObject.transform);
        }

        private void ShowNextToStatus(Transform objectTransform)
        {
            Canvas canvas = _tooltipManager.GetComponentInParent<Canvas>();

            transform.position = objectTransform.position + new Vector3((objectTransform as RectTransform).rect.width / 2 * canvas.scaleFactor, 0);
        }
    }
}
