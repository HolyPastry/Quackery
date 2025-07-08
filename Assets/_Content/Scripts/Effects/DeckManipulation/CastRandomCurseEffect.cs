using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Cast Random Curse Effect", menuName = "Quackery/Effects/Deck/Cast Random Curse Effect")]
    public class CastRandomCurseEffect : EffectData
    {
        [SerializeField] private List<Effect> _curseEffect;

        public override void Execute(Effect effect)
        {
            var curseEffect = _curseEffect[Random.Range(0, _curseEffect.Count)];
            curseEffect.Data.Execute(curseEffect);
        }
    }
}
