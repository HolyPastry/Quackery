using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using Quackery.Shops;
using UnityEditor;
using UnityEngine;

namespace Quackery.Clients
{
    public class ClientManager : Service
    {
        [SerializeField] private int _queueSize = 5;
        [SerializeField] private UnknownClientsData unknownClientsData;

        private ClientList _clientList;
        private Client _selectedClient;
        private bool _infiniteQueue;
        private Client _clientToSwap;

        void OnEnable()
        {
            ClientServices.WaitUntilReady = () => WaitUntilReady;
            ClientServices.AddKnownClient = AddKnownClient;
            ClientServices.AddUnknownClients = AddUnknownClients;

            ClientServices.HasNextClient = HasNextClient;
            ClientServices.GetNextClient = GetNextClient;
            ClientServices.GetClients = () => _clientList?.Clients;
            ClientServices.GenerateDailyQueue = GenerateDailyQueue;
            ClientServices.ClientServed = ClientServed;

            ClientServices.SelectClient = SelectClient;
            ClientServices.SelectedClient = () => _selectedClient;
            ClientServices.ClientLeaves = ClientLeaves;

            ClientServices.IsCurrentClientAnonymous = () => _selectedClient != null && _selectedClient.IsAnonymous && _clientToSwap == null;
            ClientServices.SwapCurrentClientTo = SwapCurrentClientTo;

            ClientServices.SetInfiniteQueue = (infinite) => { };
            ClientServices.GetRevealedClient = () => _clientToSwap;

            ClientServices.SwapClients = SwapClients;

        }



        void OnDisable()
        {
            ClientServices.WaitUntilReady = () => new WaitUntil(() => true);
            ClientServices.AddKnownClient = delegate { };
            ClientServices.AddUnknownClients = (num) => { };


            ClientServices.HasNextClient = () => true;
            ClientServices.GetNextClient = () => null;
            ClientServices.GetClients = () => new();
            ClientServices.GenerateDailyQueue = delegate { };
            ClientServices.ClientServed = delegate { };

            ClientServices.SelectClient = (client) => { };
            ClientServices.SelectedClient = () => null;
            ClientServices.ClientLeaves = () => { };
            ClientServices.IsCurrentClientAnonymous = () => false;
            ClientServices.SwapClients = () => { };

            ClientServices.SetInfiniteQueue = (isOn) => _infiniteQueue = isOn;
            ClientServices.GetRevealedClient = () => null;

        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _clientList = new ClientList();
            _clientList.Init();
            _isReady = true;
        }

        private void SwapClients()
        {
            _clientList.Remove(_selectedClient);
            _clientList.Add(_clientToSwap);
            _selectedClient = _clientToSwap;
        }

        private void SwapCurrentClientTo(ClientData data)
        {
            _clientToSwap = new Client();

            _clientToSwap.InitKnownClient(data);
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

            int missingClients = _queueSize;

            missingClients -= AddKnownClientsToQueue(_queueSize);


            if (missingClients > _clientList.UnknownClients.Count)
            {
                AddUnknownClients(missingClients - _clientList.UnknownClients.Count);
            }

            _clientList.Clients.Shuffle();

            for (int i = 0; i < _queueSize; i++)
            {
                if (_clientList.Clients[i].IsAnonymous)
                    _clientList.Clients[i].IsInQueue = true;
                else
                    i++;
            }
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private int AddKnownClientsToQueue(int queueSize)
        {
            var availableKnownClients =
                 _clientList.Clients.FindAll(c => !c.IsAnonymous && c.QuestFullfilled);
            foreach (var client in availableKnownClients)
            {
                client.IsInQueue = true;
            }
            return availableKnownClients.Count;
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

        private void ClientServed(Client client)
        {
            client.IsNew = false;
            client.Served = true;
            _clientList.Save();

            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        private Client GetNextClient()
        {
            var clients = _clientList.Clients.FindAll(c => c.IsInQueue && !c.Served);
            if (clients.Count == 0)
            {
                if (!_infiniteQueue)
                    return null;
                GenerateDailyQueue();
                clients = _clientList.Clients.FindAll(c => c.IsInQueue && !c.Served);
                if (clients.Count == 0)
                {
                    Debug.LogWarning("No clients available in INFINITE queue.");
                    return null;
                }
            }
            _selectedClient = clients[0];
            ClientEvents.OnClientSwap?.Invoke(clients[0]);
            return clients[0];
        }

        private bool HasNextClient()
        {
            return _clientList.Clients.Exists(c => (c.IsInQueue && !c.Served) || _infiniteQueue);
        }

        private void RemoveUnknownClient(string key)
        {
            _clientList.RemoveFromKey(key);
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void AddKnownClient(ClientData data)
        {
            var client = new Client();
            client.InitKnownClient(data);
            _clientList.Add(client);
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void AddUnknownClients(int numClients)
        {
            for (int i = 0; i < numClients; i++)
            {
                var client = new Client();
                client.InitUnknown(unknownClientsData);
                _clientList.Add(client);
                ClientEvents.ClientListUpdated?.Invoke();
            }

        }
    }
}
