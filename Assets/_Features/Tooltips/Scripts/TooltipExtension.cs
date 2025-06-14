using KBCore.Refs;
using UnityEngine;

namespace Quackery
{

    [RequireComponent(typeof(TooltipManager))]
    public abstract class TooltipExtension : ValidatedMonoBehaviour
    {
        [SerializeField, Self] protected TooltipManager _tooltipManager;

        protected RectTransform _rectTransform => transform as RectTransform;

        public abstract void SetTooltip(GameObject HoveredObject);

    }
}
