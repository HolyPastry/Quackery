using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class TooltipRequester : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            TooltipUI.ShowTooltipRequest(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //    TooltipUI.HideTooltipRequest();
        }
    }
}
