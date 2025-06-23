using KBCore.Refs;
using UnityEngine;

namespace Quackery
{

    [RequireComponent(typeof(TooltipUI))]
    public abstract class TooltipExtension : ValidatedMonoBehaviour
    {
        [SerializeField, Self] protected TooltipUI _tooltipManager;

        protected RectTransform _rectTransform => transform as RectTransform;

        public abstract void SetTooltip(GameObject HoveredObject);

    }
}
