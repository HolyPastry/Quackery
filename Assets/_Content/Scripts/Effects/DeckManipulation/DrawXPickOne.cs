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
            DeckServices.InterruptDraw();
            List<Card> drawnCards = DeckServices.Draw(effect.Value);
            DeckServices.MoveToCardSelect(drawnCards);
            DeckEvents.OnCardSelected += OnCardSelected;

            yield return new WaitUntil(() => _cardSelected != null);

            DeckEvents.OnCardSelected -= OnCardSelected;
            DeckServices.MoveToTable(_cardSelected);
            DeckServices.Discard(_otherCards);
            DeckServices.ResumeDraw();

            yield return DefaultWaitTime;


        }
        private void OnCardSelected(Card selectedCard, List<Card> otherCards)
        {
            _cardSelected = selectedCard;
            _otherCards = otherCards;
        }
    }
}
