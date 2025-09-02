using System;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;
using Quackery.Effects;


namespace Quackery.Clients
{
    [Serializable]
    public class ClientList : SerialData
    {
        [NonSerialized]
        public DataCollection<ClientData> Collection;
        [NonSerialized]
        public DataCollection<EffectData> EffectCollection;
        public List<Client> Clients;

        public List<Client> UnknownClients => Clients.FindAll(c => c.IsAnonymous);

        public void Init()
        {
            Collection = new DataCollection<ClientData>("Clients");
            EffectCollection = new DataCollection<EffectData>("Effects");
            var clientList = SaveServices.Load<ClientList>("Clients");
            if (clientList != null)
                Clients = new(clientList.Clients);
            else
                Clients = new();
        }

        internal void Save()
        {
            SaveServices.Save("Clients", this);
        }

        public override void Serialize()
        {
            base.Serialize();
        }

        public override void Deserialize()
        {
            base.Deserialize();
            foreach (var client in Clients)
            {
                client.ReconcileData(Collection);
            }
        }

        internal void Add(Client client)
        {
            if (client == null)
                return;

            Clients.AddUnique(client);
            Save();

        }


        internal bool TryAndGet(string key, out Client client)
        {
            client = null;

            client = Clients.Find(c => c.Key == key);
            return client != null;
        }

        internal void RemoveFromKey(string key)
        {
            if (Clients.RemoveAll(c => c.Key == key) > 0)
                Save();
        }

        internal void Remove(Client selectedClient)
        {
            RemoveFromKey(selectedClient.Key);
        }
    }
}
