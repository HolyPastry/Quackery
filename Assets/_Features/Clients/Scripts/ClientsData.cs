using System;
using System.Collections.Generic;
using Quackery.Effects;
using Quackery.Progression;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Quackery.Clients
{
    [Serializable]
    public struct ClientLevelData
    {
        public List<Effect> Effects;
        public int SurvivalThreshold;
        public int NormalThreshold;
        public Vector2Int QueueSizeRange;
        public int QueueSize => UnityEngine.Random.Range(QueueSizeRange.x, QueueSizeRange.y + 1);

        public int CartSize;
    }

    [CreateAssetMenu(
        fileName = "UnknownClientsData",
        menuName = "Quackery/UnknownClientsData",
        order = 0)]
    public class ClientsData : SerializedScriptableObject
    {
        public List<Sprite> Icons;
        public List<string> NamePrefixes;
        public List<string> NameSuffixes;

        [SerializeField]
        private List<ClientLevelData> LevelsData;

        public Sprite RandomIcon => Icons[Random.Range(0, Icons.Count)];

        internal List<Effect> RandomEffects => GenerateRandomEffects();

        public string RandomName
        {
            get
            {
                var prefix = NamePrefixes[Random.Range(0, NamePrefixes.Count)];
                var suffix = NameSuffixes[Random.Range(0, NameSuffixes.Count)];
                var number = Random.Range(1, 1000);
                return $"{prefix}{suffix}{number}";
            }
        }

        private List<Effect> GenerateRandomEffects()
        {
            var level = ProgressionServices.GetLevel();
            if (level >= LevelsData.Count)
            {
                Debug.LogWarning($"No client data found for level {level}");
                return new();
            }
            Effect effect = new(LevelsData[level].Effects[Random.Range(0, LevelsData[level].Effects.Count)]);

            return new() { effect };
        }

        internal int GetThreshold(CartMode mode)
        {
            int level = ProgressionServices.GetLevel();
            if (level >= LevelsData.Count)
            {
                Debug.LogWarning($"No client data found for level {level}");
                return 30; // Default threshold if not found
            }

            return mode == CartMode.Survival ?
                LevelsData[level].SurvivalThreshold : LevelsData[level].NormalThreshold;

        }

        public int GetQueueSize()
        {
            int level = ProgressionServices.GetLevel();
            if (level >= LevelsData.Count)
            {
                Debug.LogWarning($"No client data found for level {level}");
                return 2; // Default queue size if not found
            }
            return LevelsData[level].QueueSize;
        }

        internal int GetCartSize()
        {
            int level = ProgressionServices.GetLevel();
            if (level >= LevelsData.Count)
            {
                Debug.LogWarning($"No client data found for level {level}");
                return 3; // Default queue size if not found
            }
            return LevelsData[level].CartSize;
        }
    }

}
