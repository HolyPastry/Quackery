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
            foreach (var effectData in data.Effects)
                Effects.Add(new Effect(effectData, true));
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

        public string LastReviewText = "";
        public int LastRating = 0;

        public string ChatHistory;

        internal void GoodReview()
        {
            LastReviewText = "Awesome, exactly what I needed";
            LastRating = 5;
        }

        internal void BadReview()
        {
            LastReviewText = "What a Quacker, To avoid at all cost";
            LastRating = 1;
        }
    }
}
