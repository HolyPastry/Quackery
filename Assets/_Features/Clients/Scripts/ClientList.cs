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

        private void Save()
        {
            SaveServices.Save("Clients", this);
        }

        public override void Serialize()
        {
            base.Serialize();
            foreach (var client in Clients)
            {
                client.Key = client.Data.name;
            }
        }

        public override void Deserialize()
        {
            base.Deserialize();
            foreach (var client in Clients)
            {
                client.Data = Collection.GetFromName(client.Key);
                foreach (var effect in client.Effects)
                    effect.Data = EffectCollection.GetFromName(effect.Key);
            }
        }

        internal void Add(Client client)
        {
            if (client == null || client.Data == null)
                return;

            Clients.AddUnique(client);
            Save();

        }

        internal void Remove(ClientData data)
        {
            if (data == null)
                return;

            var client = Clients.Find(c => c.Data == data);
            if (client != null)
            {
                Clients.Remove(client);
                Save();
            }
        }

        internal bool TryAndGet(ClientData data, out Client client)
        {
            client = null;
            if (data == null)
                return false;
            client = Clients.Find(c => c.Data == data);
            return client != null;
        }
    }
}
