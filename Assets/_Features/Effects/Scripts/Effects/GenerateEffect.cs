

using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(
         fileName = "GenerateEffect",
         menuName = "Quackery/Effects/GenerateEffect",
         order = 1)]
    public class GenerateEffect : EffectData
    {

        public EffectData EffectData;
        public override void Execute(Effect effect, CardPile pile)
        {
            Effect newEffect = new(EffectData);
            if (!pile.IsEmpty)
            {
                newEffect.LinkedCard = pile.TopCard;
                newEffect.Origin = (Vector2)pile.TopCard.transform.position;
            }
            EffectServices.Add(newEffect);
        }
    }
}
