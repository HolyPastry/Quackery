using System;
using System.Collections.Generic;
using Bakery.Dialogs;
using Holypastry.Bakery.Quests;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Clients
{
    [CreateAssetMenu(
        fileName = "ClientData",
        menuName = "Quackery/ClientData",
        order = 1)]
    public class ClientData : ScriptableObject
    {
        [Serializable]
        public record Reward
        {
            [TextArea(3, 10)]
            public string Review;
            public int Rating;
            public int FollowerBonus;
        }
        public CharacterData CharacterData;
        public Sprite Icon;
        public List<EffectData> Effects;

        public Reward FirstReward;

        public QuestData FirstQuest;

        public string Name => CharacterData.MasterText;

        public int Budget = -1;
    }
}
