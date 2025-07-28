using System.Collections.Generic;
using Quackery.Progression;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quackery.Clients
{
    [CreateAssetMenu(
        fileName = "UnknownClientsData",
        menuName = "Quackery/UnknownClientsData",
        order = 0)]
    public class UnknownClientsData : SerializedScriptableObject
    {
        public List<Sprite> Icons;
        public List<string> NamePrefixes;
        public List<string> NameSuffixes;

        [SerializeField]
        private Dictionary<int, List<Effect>> EffectsByLevel;

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

            if (EffectsByLevel.TryGetValue(level, out var effects))
                return new() { effects[Random.Range(0, effects.Count)] };

            return new();
        }
    }
}
