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
        public EnumCardPile TargetDeck = EnumCardPile.Discard;

        public override void Execute(Effect effect)
        {
            int countered = EffectServices.CounterEffect(_counterEffect, effect.Value);
            if (TargetDeck == EnumCardPile.Draw)
            {
                DeckServices.AddNewToDrawDeck(_curseCard, effect.Value - countered);
                DeckServices.Shuffle();
            }
            else if (TargetDeck == EnumCardPile.Discard)
            {
                DeckServices.AddNewToDiscard(_curseCard, effect.Value - countered);
            }
        }
    }
}
