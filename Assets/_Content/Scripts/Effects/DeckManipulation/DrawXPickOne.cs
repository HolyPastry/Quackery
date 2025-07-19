using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "DrawXPickOne", menuName = "Quackery/Effects/Deck/Draw X Pick One", order = 1)]
    public class DrawXPickOne : EffectData
    {
        private Card _cardSelected;
        private List<Card> _otherCards;

        // public override string Description => "Draw two cards from the deck, select one to keep, and discard the other.";

        public override IEnumerator Execute(Effect effect)
        {
            _cardSelected = null;
            _otherCards = null;

            DeckServices.SetCustomDraw(effect.Value);

            yield return null;


        }
        private void OnCardSelected(Card selectedCard, List<Card> otherCards)
        {
            _cardSelected = selectedCard;
            _otherCards = otherCards;
        }
    }
}
