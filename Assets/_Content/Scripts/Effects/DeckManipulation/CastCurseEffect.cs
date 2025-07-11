using System;
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

        public override void Execute(Effect effect)
        {
            int countered = EffectServices.CounterEffect(_counterEffect, effect.Value);


            for (int i = 0; i < effect.Value - countered; i++)
            {
                DeckServices.AddNew(_curseCard,
                                        TargetDeck,
                                        Placement,
                                        Lifetime, true);
                Card card = DeckServices.CreateCard(_curseCard);
                card.Item.Lifetime = Lifetime;
                DeckServices.MoveCardToEffect(card, true);
                //DeckServices.MoveCardTo
            }
        }
    }
}
