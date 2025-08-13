using System;
using System.Collections.Generic;
using Holypastry.Bakery;
using Holypastry.Bakery.Quests;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Clients
{
    public record Client
    {
        public enum EnumState
        {
            Unknown,
            Revealed,
            Ready,
            WonOver
        }

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

        public int Budget { get; private set; }

        public bool IsNew;

        public string LastReviewText = "";
        public int LastRating = 0;

        public string ChatHistory;
        public bool IsOnline => State == EnumState.Ready;
        public EnumState State = EnumState.Unknown;

        public QuestData FirstQuest => (Data == null) ? null : Data.FirstQuest;

        public bool QuestFullfilled => FirstQuest == null || QuestServices.IsQuestCompleted(FirstQuest);

        public Condition RevealCondition { get; internal set; }

        public void InitUnknown(ClientsData unknownClientsData, Effect effect = null)
        {

            Key = unknownClientsData.RandomName;
            if (effect != null)
                Effects = new List<Effect> { effect };
            else
                Effects = unknownClientsData.RandomEffects;


            Portrait = unknownClientsData.RandomIcon;
            LoginName = Key;
            Budget = -1;
            IsNew = true;
            ChatHistory = string.Empty;

            InitEffects();

        }

        private void InitEffects()
        {
            foreach (var effect in Effects)
            {
                effect.Initialize();
                effect.Tags.AddUnique(Quackery.Effects.EnumEffectTag.Client);
                effect.Tags.AddUnique(Quackery.Effects.EnumEffectTag.Status);
            }
        }

        public void InitKnownClient(ClientData data)
        {

            Data = data;
            Key = data.name;
            Budget = data.Budget;

            if (FirstQuest != null)
                RevealCondition = FirstQuest.Steps?[1].Conditions?[0];

            State = EnumState.Revealed;

            if (data.Effects != null)
                Effects = new List<Effect>(data.Effects);
            else
                Effects = new List<Effect>();

            LoginName = data.CharacterData.MasterText;
            Portrait = data.Icon;
            LastReviewText = data.FirstReward.Review;
            LastRating = data.FirstReward.Rating;
            LastFollowersBonus = data.FirstReward.FollowerBonus;
            InitEffects();
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
