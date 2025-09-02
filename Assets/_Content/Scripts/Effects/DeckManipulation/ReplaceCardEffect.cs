using System;
using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ReplaceCard", menuName = "Quackery/Effects/Deck/Replace Card")]
    public class ReplaceCardEffect : EffectData, IValueEffect
    {
        public enum EnumCondition
        {
            NumberOfDraws
        }

        [SerializeField] private EnumCondition _condition;
        [SerializeField] private ItemData _replacementCard;
        [SerializeField] private int _numberDraws;

        public float Value => _numberDraws;

        public override IEnumerator Execute(Effect effect)
        {

            Card card = effect.LinkedObject as Card;

            if (card == null ||
                _condition != EnumCondition.NumberOfDraws ||
                 card.Item.NumberOfDraws < effect.Value)
                yield break;

            yield return DeckServices.MoveCard(card, EnumCardPile.Effect, EnumPlacement.OnTop, Tempo.WholeBeat);
            yield return Tempo.WaitForABeat;

            Card newCard = DeckServices.AddNew(_replacementCard,
                                    EnumCardPile.Effect,
                                    EnumPlacement.OnTop,
                                    EnumLifetime.Permanent);
            yield return Tempo.WaitForABeat;
            yield return DeckServices.DestroyCard(card);

            yield return DeckServices.MoveCard(newCard, EnumCardPile.Discard, EnumPlacement.OnTop, Tempo.WholeBeat);
        }
    }
}
