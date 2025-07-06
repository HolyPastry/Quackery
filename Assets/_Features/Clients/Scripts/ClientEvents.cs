using System;

namespace Quackery.Clients
{
    public static class ClientEvents
    {
        public static Action ClientListUpdated = delegate { };

        public static Action<Client> OnClientSwap = delegate { };

        public static Action<Client> OnClientServed = delegate { };
    }
}
