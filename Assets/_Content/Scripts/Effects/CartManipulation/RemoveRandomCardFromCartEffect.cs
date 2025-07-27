using System.Collections;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "RemoveRandomCardFromCart", menuName = "Quackery/Effects/Cart/Remove Random Card", order = 0)]
    public class RemoveRandomCardFromCartEffect : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            var cards = DeckServices.GetMatchingCards(c => true,
                                                       EnumCardPile.Cart);

            //   int numStatuses = EffectServices.GetNumStatuses() - 1; // Subtract 1 for the effect itself
            int numStatuses = 1;
            int numCardsToRemove = Mathf.Min(numStatuses, cards.Count);
            while (numCardsToRemove > 0)
            {
                var randomCard = cards[Random.Range(0, cards.Count)];
                CartServices.AddToCartValue(-randomCard.Price);
                DeckServices.Discard(new() { randomCard });
                cards.Remove(randomCard);
                numCardsToRemove--;
                yield return DefaultWaitTime;
            }

        }
    }
}
