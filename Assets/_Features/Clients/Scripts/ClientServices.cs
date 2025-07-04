using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Clients
{
    public static class ClientServices
    {

        internal static Func<bool> HasNextClient = () => true;
        internal static Action<ClientData> AddKnownClient = delegate { };
        internal static Action<int> AddUnknownClients = (count) => { };

        public static Func<List<Client>> GetClients = () => new();

        public static Func<Client> GetNextClient = () => null;
        public static Action<Client> ClientServed = (client) => { };

        public static Action GenerateDailyQueue = delegate { };

        internal static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);

        internal static Action<Client> SelectClient = (client) => { };
        public static Func<Client> SelectedClient = () => null;

        internal static Action ClientLeaves = () => { };

        internal static Action<bool> SetInfiniteQueue = (infinite) => { };
    }
}
