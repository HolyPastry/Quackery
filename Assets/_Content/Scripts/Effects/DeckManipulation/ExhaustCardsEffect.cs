using System.Collections;
using Quackery.Decks;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "ExhaustCardsEffect", menuName = "Quackery/Effects/Deck/Exhaust Cards", order = 0)]
    public class ExhaustCardsEffect : EffectData, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category;
        [SerializeField] private EnumCardPile _targetPile;
        public EnumItemCategory Category => _category;



        public override IEnumerator Execute(Effect effect)
        {
            bool predicate(Card card) => card.Category == _category || _category == EnumItemCategory.Any;

            var cards = DeckServices.GetMatchingCards(predicate, _targetPile);

            foreach (var card in cards)
            {
                DeckServices.MoveCard(card, EnumCardPile.Effect, EnumPlacement.OnTop, 0);
                DeckServices.MoveCard(card, EnumCardPile.Exhaust, EnumPlacement.OnTop, 2);
                yield return DefaultWaitTime;
            }
        }
    }
}
