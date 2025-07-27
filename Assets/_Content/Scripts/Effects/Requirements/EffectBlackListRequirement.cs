
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "EffectBlackListRequirement", menuName = "Quackery/Effects/Requirements/Black List", order = 0)]
    public class EffectBlackListRequirement : EffectData, IEffectRequirement
    {
        [SerializeField] private List<EffectData> _blackList = new();
        public bool IsFulfilled(Effect effect, Card card)
        {
            return card.Effects.TrueForAll(e =>
            {
                foreach (var blackListedEffect in _blackList)
                {
                    if (e.Data == blackListedEffect)
                    {
                        return false; // Effect is blacklisted
                    }
                }
                return true; // Effect is not blacklisted
            });
        }
    }
}
