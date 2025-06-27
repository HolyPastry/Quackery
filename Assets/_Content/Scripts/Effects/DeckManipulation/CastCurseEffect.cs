using Ink.Runtime;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "Cast Curse Effect", menuName = "Quackery/Effects/Cast Curse Effect")]
    public class CastCurseEffect : EffectData
    {
        [SerializeField] private EffectData _counterEffect;
        [SerializeField] private ItemData _curseCard;
        public EnumPileType TargetDeck = EnumPileType.DiscardPile;

        public override void Execute(Effect effect, CardPile drawPile)
        {
            int countered = EffectServices.CounterEffect(_counterEffect, effect.Value);
            if (TargetDeck == EnumPileType.DrawPile)
            {
                DeckServices.AddNewToDrawDeck(_curseCard, effect.Value - countered);
                DeckServices.Shuffle();
            }
            else if (TargetDeck == EnumPileType.DiscardPile)
            {
                DeckServices.AddNewToDiscard(_curseCard, effect.Value - countered);
            }
        }
    }
}
