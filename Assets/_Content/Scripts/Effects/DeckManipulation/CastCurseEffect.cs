using System;
using System.Collections;
using Ink.Runtime;
using Quackery.Decks;
using Quackery.GameStats;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "Cast Curse Effect", menuName = "Quackery/Effects/Deck/Cast Curse Effect")]
    public class CastCurseEffect : EffectData
    {
        [SerializeField] private EffectData _counterEffect;
        [SerializeField] private ItemData _curseCard;

        public EnumCardPile TargetDeck = EnumCardPile.Discard;
        public EnumLifetime Lifetime = EnumLifetime.Permanent;
        public EnumPlacement Placement = EnumPlacement.OnTop;

        public override IEnumerator Execute(Effect effect)
        {
            int countered = EffectServices.CounterEffect(_counterEffect, (int)effect.Value);


            for (int i = 0; i < effect.Value - countered; i++)
            {
                Card card = DeckServices.AddNew(_curseCard,
                                        EnumCardPile.Effect,
                                       Placement,
                                        Lifetime);

                DeckServices.MoveCard(card, TargetDeck, Placement, 2f);
                yield return new WaitForSeconds(2f);
            }
        }
    }
}
