using System;

namespace Quackery
{
    public record ClientInfo
    {
        [NonSerialized]
        public ClientData Data;
        public string Key;

        public ClientInfo()
        { }

        public ClientInfo(ClientData data)
        {
            Data = data;
            Key = data.name;
        }

    }
}
