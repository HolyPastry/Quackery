using System;
using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "ReplaceCard", menuName = "Quackery/Effects/Deck/Replace Card")]
    public class ReplaceCard : EffectData
    {
        public enum EnumCondition
        {
            NumberOfDraws
        }

        [SerializeField] private EnumCondition _condition;
        [SerializeField] private ItemData _replacementCard;
        // [SerializeField] private EnumPlacement _placement = EnumPlacement.OnTop;
        // [SerializeField] private EnumLifetime _lifetime = EnumLifetime.Permanent;
        // [SerializeField] private EnumCardPile _targetDeck = EnumCardPile.Discard;
        public override IEnumerator Execute(Effect effect)
        {



            Card card = effect.LinkedObject as Card;
            if (card == null) yield break;
            Item item = card.Item;

            if (_condition == EnumCondition.NumberOfDraws && item.NumberOfDraws >= effect.Value)
            {
                DeckServices.ReplaceCard(card, _replacementCard);
            }
            yield return DefaultWaitTime;
        }
    }
}
