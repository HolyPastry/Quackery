using UnityEngine;
using UnityEngine.EventSystems;

namespace Quackery
{
    public class TooltipRequester : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipManager.ShowTooltipRequest(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.HideTooltipRequest();
        }
    }
}
