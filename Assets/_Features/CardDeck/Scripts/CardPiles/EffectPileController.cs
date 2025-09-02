

using System.Collections;

using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class EffectPileController : CardPilePool, IPileController
    {
        public EnumCardPile CardPileType => _cardPileType;

        public void Teleport(Card card)
        {
            CardPile pile = GetEmptyPile(expandIfFull: true);
            pile.AddOnTop(card, isInstant: true);
        }

        public IEnumerator Move(Card card)
        {
            CardPile pile = GetEmptyPile(expandIfFull: true);
            pile.AddOnTop(card, isInstant: false);
            yield return Tempo.WaitForABeat;
        }

        public override bool RemoveCard(Card card)
        {
            bool cardRemoved = base.RemoveCard(card);
            DisableEmptyPiles();
            return cardRemoved;
        }
    }
}
