using System;
using System.Collections.Generic;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Clients
{
    public static class ClientServices
    {

        internal static Func<bool> HasNextClient = () => true;
        internal static Action<ClientData> AddKnownClient = delegate { };
        internal static Action<int> AddUnknownClients = (count) => { };

        public static Func<List<Client>> GetVIPClients = () => new();

        public static Func<Client> GetNextClient = () => null;
        public static Action<Client, bool> ClientServed = (client, success) => { };

        public static Action GenerateDailyQueue = delegate { };

        internal static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);

        internal static Action<Client> SelectVIPClient = (client) => { };
        public static Func<Client> SelectedClient = () => null;

        internal static Action ClientLeaves = () => { };

        internal static Action<bool> SetInfiniteQueue = (infinite) => { };

        internal static Action<ClientData> SwapCurrentClientTo = (clientData) => { };

        internal static Func<bool> IsCurrentClientAnonymous = () => false;

        internal static Func<Client> GetRevealedClient = () => null;

        internal static Action SwapClients = () => { };

        internal static Action<EffectData> AddUnknownClient = (effect) => { };

        internal static Func<int> GetBudget = () => -1;

        internal static Action<ClientData, Client.EnumState> SetClientState = (clientData, state) => { };

        internal static Func<ClientData, Client.EnumState, bool> CheckStatus = (clientData, state) => false;

        internal static Func<int> NumClientsToday = () => 1;

        internal static Func<int> GetQueueSize = () => 0;
        internal static Action StartNormalWeek = () => { };

        internal static Func<List<Client>> GetClientList = () => new();

        internal static Func<CartMode, int> GetThreshold = (mode) => (mode == CartMode.Survival) ? 20 : 40;

        internal static Func<int> GetCartSize = () => 3;

        internal static Func<Client> GetClient = () => null;

    }
}
