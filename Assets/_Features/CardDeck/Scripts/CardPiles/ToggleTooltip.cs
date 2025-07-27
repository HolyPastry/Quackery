using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;


namespace Quackery.Decks
{
    public class ToggleTooltip : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
    {
        private bool _isOn;


        // void Start()
        // {
        //     TooltipUI.OnShowTooltip += UpdateTooltipState;
        // }
        // void OnDestroy()
        // {
        //     TooltipUI.OnShowTooltip -= UpdateTooltipState;
        // }

        private void UpdateTooltipState(GameObject @object)
        {
            if (@object != gameObject)
                _isOn = false;
        }



        private void Toggle()
        {
            if (_isOn)
                TooltipUI.HideTooltipRequest();
            else

                TooltipUI.ShowTooltipRequest(gameObject);

            _isOn = !_isOn;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Toggle();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }
    }
}
