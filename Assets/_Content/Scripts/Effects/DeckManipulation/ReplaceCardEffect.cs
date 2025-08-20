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
            if (card == null) yield break;
            Item item = card.Item;

            if (_condition == EnumCondition.NumberOfDraws && item.NumberOfDraws >= effect.Value)
                yield return DeckServices.ReplaceCard(card, _replacementCard);

        }
    }
}
