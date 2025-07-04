using System.Collections.Generic;

using UnityEngine;

namespace Quackery.Clients
{
    [CreateAssetMenu(
        fileName = "UnknownClientsData",
        menuName = "Quackery/UnknownClientsData",
        order = 0)]
    public class UnknownClientsData : ScriptableObject
    {
        public List<Sprite> Icons;
        public List<string> NamePrefixes;
        public List<string> NameSuffixes;
        public List<Effect> Effects;

        public Sprite RandomIcon => Icons[Random.Range(0, Icons.Count)];

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

        internal List<Effect> RandomEffects
        => new() { Effects[Random.Range(0, Effects.Count)] };


    }
}
