using System;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;

namespace Quackery
{
    [Serializable]
    public class ClientList : SerialData
    {
        [NonSerialized]
        public DataCollection<ClientData> Collection;
        public readonly List<ClientInfo> Clients;

        public ClientList()
        {
            Collection = new DataCollection<ClientData>("Clients");
            var clientList = SaveServices.Load<ClientList>("Clients");
            if (clientList != null)
                Clients = clientList.Clients;
            else
                Clients = new();
        }

        public void Save()
        {
            SaveServices.Save("Clients", this);
        }

        public void Load()
        {

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
            }
        }


    }
}
