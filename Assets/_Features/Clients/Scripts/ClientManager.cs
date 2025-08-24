using System;
using System.Collections;
using Holypastry.Bakery.Flow;
using Quackery.Effects;
using Quackery.Followers;
using Quackery.Progression;
using UnityEngine;

namespace Quackery.Clients
{
    public class ClientManager : Service
    {
        [SerializeField] private int _queueSize = 5;
        [SerializeField] private ClientsData _clientsData;

        private ClientList _clientList;
        private Client _selectedClient;
        private Client _clientToSwap;
        private bool _infiniteQueue;

        void OnEnable()
        {
            ClientServices.WaitUntilReady = () => WaitUntilReady;
            ClientServices.AddKnownClient = AddKnownClient;
            ClientServices.AddUnknownClients = AddUnknownClients;
            ClientServices.GetClient = () => _selectedClient;

            ClientServices.HasNextClient = HasNextClient;
            ClientServices.GetNextClient = GetNextClient;
            ClientServices.GetVIPClients = () => _clientList?.Clients.FindAll(c => !c.IsAnonymous);
            ClientServices.GenerateDailyQueue = GenerateDailyQueue;
            ClientServices.ClientServed = ClientServed;

            ClientServices.SelectVIPClient = SelectVIPClient;
            ClientServices.SelectedClient = () => _selectedClient;
            ClientServices.ClientLeaves = ClientLeaves;

            ClientServices.IsCurrentClientAnonymous = () => _selectedClient != null && _selectedClient.IsAnonymous && _clientToSwap == null;
            ClientServices.SwapCurrentClientTo = SwapCurrentClientTo;

            ClientServices.SetInfiniteQueue = (infinite) => { };
            ClientServices.GetRevealedClient = () => _clientToSwap;
            ClientServices.AddUnknownClient = AddUnknownClient;

            ClientServices.NumClientsToday = () => _queueSize;

            ClientServices.SwapClients = SwapClients;
            ClientServices.GetBudget = () => _selectedClient?.Budget ?? -1;
            ClientServices.SetClientState = SetClientState;
            ClientServices.CheckStatus = CheckClientStatus;

            ClientServices.GetClientList = () => _clientList?.Clients ?? new();


            ClientServices.GetQueueSize = () => _queueSize;
            ClientServices.StartNormalWeek = StartNormalWeek;
            ClientServices.GetThreshold = (mode) => _clientsData.GetThreshold(mode);
            ClientServices.GetCartSize = _clientsData.GetCartSize;

        }



        void OnDisable()
        {
            ClientServices.WaitUntilReady = () => new WaitUntil(() => true);
            ClientServices.AddKnownClient = delegate { };
            ClientServices.AddUnknownClients = (num) => { };


            ClientServices.HasNextClient = () => true;
            ClientServices.GetNextClient = () => null;
            ClientServices.GetVIPClients = () => new();
            ClientServices.GenerateDailyQueue = delegate { };
            ClientServices.ClientServed = delegate { };

            ClientServices.SelectVIPClient = (client) => { };
            ClientServices.SelectedClient = () => null;
            ClientServices.ClientLeaves = () => { };

            ClientServices.IsCurrentClientAnonymous = () => false;
            ClientServices.SwapCurrentClientTo = (data) => { };

            ClientServices.SetInfiniteQueue = (isOn) => _infiniteQueue = isOn;
            ClientServices.GetRevealedClient = () => null;
            ClientServices.SwapClients = () => { };
            ClientServices.AddUnknownClient = (effect) => { };

            ClientServices.NumClientsToday = () => 0;

            ClientServices.SetClientState = (clientData, state) => { };
            ClientServices.CheckStatus = (clientData, state) => false;

            ClientServices.GetClientList = () => new();


            ClientServices.GetBudget = () => -1;
            ClientServices.GetQueueSize = () => 0;
            ClientServices.StartNormalWeek = () => { };
            ClientServices.GetThreshold = (mode) => (mode == CartMode.Survival) ? 20 : 40;
            ClientServices.GetCartSize = () => 3;

        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _clientList = new ClientList();
            _clientList.Init();
            _isReady = true;
        }



        private int GetCartThreshold(CartMode mode)
        {
            return _clientsData.GetThreshold(mode);
        }

        private void SwapClients()
        {
            _clientList.Remove(_selectedClient);
            _selectedClient = _clientToSwap;
        }

        private void SwapCurrentClientTo(ClientData data)
        {
            _clientToSwap = new Client();
            _clientList.Add(_clientToSwap);
            _clientToSwap.InitKnownClient(data);

        }
        private bool CheckClientStatus(ClientData data, Client.EnumState state)
        {

            var client = _clientList.Clients.Find(c => c.Key == data.name);
            if (client == null)
            {
                Debug.LogWarning($"Client with key {data.name} not found in the client list.");
                return false;
            }

            return client.State == state;
        }
        private void SetClientState(ClientData data, Client.EnumState state)
        {
            switch (state)
            {
                case Client.EnumState.Unknown:
                    Debug.LogWarning("Cannot set client state to Unknown. This should not happen.");
                    break;
                case Client.EnumState.Revealed:
                    SwapCurrentClientTo(data);
                    break;
                case Client.EnumState.Ready:
                    var client = _clientList.Clients.Find(c => c.Key == data.name);
                    if (client != null)
                        client.State = Client.EnumState.Ready;
                    break;
                case Client.EnumState.WonOver:
                    client = _clientList.Clients.Find(c => c.Key == data.name);
                    if (client != null)
                        client.State = Client.EnumState.WonOver;

                    break;
            }
        }

        private void StartNormalWeek()
        {
            _selectedClient = GetNextClient();
        }

        private void SelectVIPClient(Client client)
        {
            if (client == null)
            {
                _selectedClient = null;
                return;
            }
            _clientList.Clients.ForEach(c => c.IsInQueue = false);
            client.IsInQueue = true;
            _selectedClient = GetNextClient();
        }

        private void ClientLeaves()
        {
            _selectedClient = null;
        }

        private void GenerateDailyQueue()
        {
            ResetClientQueue();

            _queueSize = _clientsData.GetQueueSize();

            AddUnknownClients(_queueSize - _clientList.UnknownClients.Count);
            _clientList.Clients.Shuffle();
            for (int i = 0; i < _clientList.Clients.Count; i++)
            {
                if (!_clientList.Clients[i].IsAnonymous) continue;
                _clientList.Clients[i].IsInQueue = true;
            }

            ClientEvents.ClientListUpdated?.Invoke();
        }

        private int AddKnownClientsToQueue(int queueSize)
        {
            var availableKnownClients =
                 _clientList.Clients.FindAll(c => !c.IsAnonymous
                    && c.State == Client.EnumState.Ready);

            foreach (var client in availableKnownClients)
                client.IsInQueue = true;

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

        private void ClientServed(Client client, bool succeeded)
        {
            client.IsNew = false;
            client.Served = true;
            if (succeeded)
                client.State = Client.EnumState.WonOver;

            _clientList.Save();
            ClientEvents.OnClientServed?.Invoke(client);

            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        private Client GetNextClient()
        {
            _clientToSwap = null;
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

        private void AddUnknownClient(EffectData effect)
        {
            var client = new Client();
            client.InitUnknown(_clientsData, effect);
            _clientList.Add(client);
            ClientEvents.ClientListUpdated?.Invoke();
        }

        private void AddUnknownClients(int numClients)
        {
            for (int i = 0; i < numClients; i++)
            {
                var client = new Client();
                client.InitUnknown(_clientsData);
                _clientList.Add(client);
                ClientEvents.ClientListUpdated?.Invoke();
            }

        }
    }
}
