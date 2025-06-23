using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class TooltipRequester : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipUI.ShowTooltipRequest(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipUI.HideTooltipRequest();
        }
    }
}
