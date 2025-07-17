using Quackery.Clients;
using UnityEngine;

namespace Quackery
{
    public class VIPListUI : MonoBehaviour
    {
        [SerializeField] private VIPPanelUI _vipPanelPrefab;
        [SerializeField] private Transform _container;
        private bool _initialized;
        void OnEnable()
        {
            ClientServices.SelectVIPClient(null);
            ClientEvents.ClientListUpdated += UpdateList;
            UpdateList();
        }

        void OnDisable()
        {
            ClientEvents.ClientListUpdated -= UpdateList;
        }

        private void UpdateList()
        {
            var clients = ClientServices.GetVIPClients();
            foreach (Transform child in _container)
            {
                if (!child.TryGetComponent<VIPPanelUI>(out var clientPanel))
                    continue;

                clientPanel.OnSelected -= OnClientSelected;
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);

            }
            foreach (var client in clients)
            {
                var clientPanel = Instantiate(_vipPanelPrefab, _container);
                clientPanel.Client = client;
                clientPanel.OnSelected += OnClientSelected;
            }
        }

        private void OnClientSelected(Client client)
        {
            ClientServices.SelectVIPClient(client);
        }
    }
}
