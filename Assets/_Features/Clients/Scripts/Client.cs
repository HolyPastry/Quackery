using System;
using System.Collections.Generic;
using Holypastry.Bakery;
using UnityEngine;

namespace Quackery.Clients
{
    public record Client
    {
        [NonSerialized]
        private ClientData Data;
        public string Key;

        public bool IsInQueue;
        public bool Served;

        public List<Effect> Effects;

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

        public string DialogName => (Data != null) ? Data.CharacterData.name : "Client";

        public bool IsAnonymous => Data == null;

        public int LastFollowersBonus = 150;

        public Sprite Portrait;
        public string LoginName;
        public object DialogKey;

        public bool IsNew;

        public string LastReviewText = "";
        public int LastRating = 0;

        public string ChatHistory;

        public void InitUnknown(UnknownClientsData unknownClientsData)
        {

            Key = unknownClientsData.RandomName;
            Effects = unknownClientsData.RandomEffects;
            Portrait = unknownClientsData.RandomIcon;
            LoginName = Key;

            IsNew = true;
            ChatHistory = string.Empty;

        }

        public void InitKnownClient(ClientData data)
        {

            Data = data;
            Key = data.name;

            if (data.Effects != null)
                Effects = new List<Effect>(data.Effects);
            else
                Effects = new List<Effect>();

            LoginName = data.CharacterData.MasterText;
            Portrait = data.Icon;
            LastReviewText = data.FirstReward.Review;
            LastRating = data.FirstReward.Rating;
            LastFollowersBonus = data.FirstReward.FollowerBonus;
        }

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

        internal void ReconcileData(DataCollection<ClientData> collection)
        {
            Data = collection.GetFromName(Key);
        }
    }
}
