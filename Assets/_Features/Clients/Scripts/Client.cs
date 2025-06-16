using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Clients
{
    public record Client
    {
        [NonSerialized]
        public ClientData Data;
        public string Key;

        public Client()
        { }

        public Client(ClientData data)
        {
            Data = data;
            Key = data.name;
        }

        public bool IsInQueue;
        public bool Served;

        public List<Effect> Effects = new();

        public string ChatLastLine
        {
            get
            {
                if (string.IsNullOrEmpty(ChatHistory))
                    return string.Empty;
                string chat = "";
                string[] chatLines = ChatHistory.Split('\n');
                if (chatLines.Length > 0)
                {
                    chat = chatLines[^1]; // Get the last line
                }
                return chat.Trim();
            }
        }

        public Sprite Portrait => Data.Icon;
        public string LoginName => Data.CharacterData.MasterText;

        public object DialogKey => Data.CharacterData.name;

        public string ChatHistory;
    }
}
