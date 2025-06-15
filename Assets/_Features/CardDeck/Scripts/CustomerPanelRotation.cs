using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{

    public enum CustomerPanelState
    {
        ReadyToEnter,
        Active,
        Exited
    }
    public class CustomerPanelRotation : MonoBehaviour
    {
        private List<CustomerPanel> _panels = new();
        private CustomerPanel _enteringPanel;

        void Awake()
        {
            GetComponentsInChildren(true, _panels);
            _panels.ForEach(p => p.TeleportToPosition(CustomerPanelState.ReadyToEnter));

        }
        internal void ClientEnter()
        {

            CustomerPanel panel = _panels.Find(p => p.CurrentState == CustomerPanelState.ReadyToEnter);
            if (panel == null)
            {
                panel = _panels.Find(p => p.CurrentState == CustomerPanelState.Exited);
                if (panel == null)
                {
                    Debug.LogError("No available customer panel found!");
                    return;
                }
                panel.TeleportToPosition(CustomerPanelState.ReadyToEnter);
            }
            _enteringPanel = panel;
            panel.SlideToPosition(CustomerPanelState.Active);
        }

        internal void ClientExit()
        {
            CustomerPanel panel = _panels.Find(p => p.CurrentState == CustomerPanelState.Active);
            if (panel == null)
            {
                Debug.LogError("No active customer panel found!");
                return;
            }
            panel.SlideToPosition(CustomerPanelState.Exited);
        }

        internal WaitUntil WaitUntilClientCameIn()
        {

            return new WaitUntil(() => _enteringPanel != null && _enteringPanel.IsPanelMoving == false);
        }
    }
}
