using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Quackery.Clients;
using Unity.VisualScripting;
using UnityEngine;

namespace Quackery
{

    public enum CustomerPanelState
    {
        ReadyToEnter,
        Active,
        Exited
    }

    public enum CustomerPanelSize
    {
        Short,
        Long
    }
    public class ClientChatRotatingPanels : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Animatable _animatable;
        private List<ClientChatInfo> _panels = new();
        private ClientChatInfo _enteringPanel;

        [SerializeField] private float _shortBottomMargin = 1900;
        [SerializeField] private float _longBottomMargin = 300;

        public Action OnExitingChat = delegate { };

        void Awake()
        {
            GetComponentsInChildren(true, _panels);

            // _panels.ForEach(p => p.TeleportToPosition(CustomerPanelState.ReadyToEnter));

        }

        void OnEnable()
        {
            ClientEvents.OnClientSwap += ClientSwap;
            _panels.ForEach(p =>
            {
                p.OnBackButtonPressed += PressBackButton;
            });

        }
        void OnDisable()
        {
            ClientEvents.OnClientSwap -= ClientSwap;
            _panels.ForEach(p =>
            {
                p.OnBackButtonPressed -= PressBackButton;

            });

        }

        private void PressBackButton()
        {
            OnExitingChat?.Invoke();
        }

        public void Show(CustomerPanelSize size)
        {
            SetSize(size);
            _canvas.gameObject.SetActive(true);
            _animatable.SlideIn();
            ClientChatInfo panel = _panels.Find(p => p.CurrentState == CustomerPanelState.Active);
            panel.EnableChat();
            panel.SetClientInfo(ClientServices.SelectedClient());
        }
        public void Hide()
        {
            _animatable.SlideOut().OnComplete(() =>
            {
                _canvas.gameObject.SetActive(false);
            });
        }

        public void SetSize(CustomerPanelSize size)
        {
            foreach (var panel in _panels)
            {

                panel.BottomMargin = size == CustomerPanelSize.Short ?
                        _shortBottomMargin : _longBottomMargin;
            }
        }
        internal void ClientSwapIn(Client client)
        {

            ClientChatInfo panel = _panels.Find(p => p.CurrentState == CustomerPanelState.ReadyToEnter);
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
            panel.SetClientInfo(client);
            panel.SlideToPosition(CustomerPanelState.Active);
            _enteringPanel = panel;

        }

        internal void ClientSwapOut()
        {
            ClientChatInfo panel = _panels.Find(p => p.CurrentState == CustomerPanelState.Active);
            if (panel == null)
            {
                Debug.LogWarning("No active customer panel found!");
                return;
            }
            panel.SlideToPosition(CustomerPanelState.Exited);
        }

        private void ClientSwap(Client client)
        {

            StartCoroutine(ClientSwapRoutine(client));
        }

        private IEnumerator ClientSwapRoutine(Client client)
        {
            _enteringPanel = null;
            ClientSwapOut();
            yield return new WaitForSeconds(0.1f);
            ClientSwapIn(client);
        }

        internal WaitUntil WaitUntilClientCameIn()
        {

            return new WaitUntil(() => _enteringPanel != null && _enteringPanel.IsPanelMoving == false);
        }
    }
}
