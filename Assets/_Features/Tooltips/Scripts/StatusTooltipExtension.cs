using UnityEngine;

namespace Quackery
{
    public class StatusTooltipExtension : TooltipExtension
    {
        public override void SetTooltip(GameObject hoveredObject)
        {
            if (!hoveredObject.TryGetComponent(out StatusUI statusUI)) return;

            _tooltipManager.AddTooltip(statusUI.Status.Data.Description);

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
