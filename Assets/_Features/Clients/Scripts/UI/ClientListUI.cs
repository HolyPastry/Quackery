using System;
using System.Collections;
using Quackery.Clients;

using UnityEngine;

namespace Quackery
{
    public class ClientListUI : MonoBehaviour
    {
        [SerializeField] private ClientPanelUI _clientPanelPrefab;
        [SerializeField] private Transform _clientListContainer;
        private bool _initialized;
        void OnEnable()
        {
            ClientServices.SelectClient(null);
            ClientEvents.ClientListUpdated += UpdateClientList;
            UpdateClientList();
        }

        void OnDisable()
        {
            ClientEvents.ClientListUpdated -= UpdateClientList;
        }

        private void UpdateClientList()
        {
            var clients = ClientServices.GetClients();
            foreach (Transform child in _clientListContainer)
            {
                if (!child.TryGetComponent<ClientPanelUI>(out var clientPanel))
                    continue;

                clientPanel.OnSelected -= OnClientSelected;
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);

            }
            foreach (var client in clients)
            {
                var clientPanel = Instantiate(_clientPanelPrefab, _clientListContainer);
                clientPanel.Client = client;
                clientPanel.OnSelected += OnClientSelected;
            }
        }

        private void OnClientSelected(Client client)
        {
            ClientServices.SelectClient(client);
        }
    }
}
