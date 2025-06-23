using System;
using System.Collections;
using Holypastry.Bakery.Flow;

using UnityEngine;

namespace Quackery.Clients
{
    public class ClientManager : Service
    {
        private ClientList _clientList;
        [SerializeField] private int _queueSize = 5;
        private Client _selectedClient;


        void OnEnable()
        {
            ClientServices.WaitUntilReady = () => WaitUntilReady;
            ClientServices.AddClient = AddClient;
            ClientServices.RemoveClient = RemoveClient;

            ClientServices.HasNextClient = HasNextClient;
            ClientServices.GetNextClient = GetNextClient;
            ClientServices.GetClients = () => _clientList?.Clients;
            ClientServices.GenerateDailyQueue = GenerateDailyQueue;
            ClientServices.ClientServed = ClientServed;

            ClientServices.SelectClient = SelectClient;
            ClientServices.SelectedClient = () => _selectedClient;
            ClientServices.ClientLeaves = ClientLeaves;

        }


        void OnDisable()
        {
            ClientServices.WaitUntilReady = () => new WaitUntil(() => true);
            ClientServices.AddClient = delegate { };
            ClientServices.RemoveClient = delegate { };

            ClientServices.HasNextClient = () => true;
            ClientServices.GetNextClient = () => null;
            ClientServices.GetClients = () => new();
            ClientServices.GenerateDailyQueue = delegate { };
            ClientServices.ClientServed = delegate { };
            ClientServices.SelectClient = (client) => { };
            ClientServices.SelectedClient = () => null;
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _clientList = new ClientList();
            _clientList.Init();
            _isReady = true;
        }

        private void SelectClient(Client client)
        {
            _selectedClient = client;
        }

        private void ClientLeaves()
        {
            _selectedClient = null;
        }

        private void GenerateDailyQueue()
        {
            ResetClientQueue();
            if (_clientList.Clients.Count < _queueSize)
                Debug.LogWarning("Not enough clients to generate a daily queue.");

            var queueSize = Mathf.Min(_queueSize, _clientList.Clients.Count);

            _clientList.Clients.Shuffle();

            for (int i = 0; i < queueSize; i++)
                _clientList.Clients[i].IsInQueue = true;
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void ResetClientQueue()
        {
            foreach (var client in _clientList.Clients)
            {
                client.IsInQueue = false;
                client.Served = false;

            }
            _selectedClient = null;
        }

        private void ClientServed(ClientData data)
        {
            if (_clientList.TryAndGet(data, out Client client))
            {
                client.IsNew = false;
                client.Served = true;
            }
            else
                Debug.LogWarning($"Client {data.name} not found in the client list.");
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private Client GetNextClient()
        {
            var clients = _clientList.Clients.FindAll(c => c.IsInQueue && !c.Served);
            if (clients.Count == 0) return null;
            _selectedClient = clients[0];
            ClientEvents.OnClientSwap?.Invoke(clients[0]);
            return clients[0];
        }

        private bool HasNextClient()
        {
            return _clientList.Clients.Exists(c => c.IsInQueue && !c.Served);
        }

        private void RemoveClient(ClientData data)
        {
            _clientList.Remove(data);
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void AddClient(ClientData data)
        {
            var client = new Client(data);
            _clientList.Add(client);
            ClientEvents.ClientListUpdated?.Invoke();
        }
    }
}
