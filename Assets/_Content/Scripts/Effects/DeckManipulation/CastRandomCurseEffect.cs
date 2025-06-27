using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Cast Random Curse Effect", menuName = "Quackery/Effects/Cast Random Curse Effect")]
    public class CastRandomCurseEffect : EffectData
    {
        [SerializeField] private List<Effect> _curseEffect;

        public override void Execute(Effect effect, CardPile drawPile)
        {
            var curseEffect = _curseEffect[Random.Range(0, _curseEffect.Count)];
            curseEffect.Data.Execute(curseEffect, drawPile);
        }
    }
}
