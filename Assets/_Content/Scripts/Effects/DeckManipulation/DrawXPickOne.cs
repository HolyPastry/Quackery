using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "DrawXPickOne", menuName = "Quackery/Effects/Draw X Pick One", order = 1)]
    public class DrawXPickOne : EffectData
    {
        // public override string Description => "Draw two cards from the deck, select one to keep, and discard the other.";

        public override void Execute(Effect effect)
        {
            DeckServices.InterruptDraw();
            List<Card> drawnCards = DeckServices.Draw(effect.Value);
            DeckServices.MoveToCardSelect(drawnCards);
            DeckEvents.OnCardSelected += OnCardSelected;

        }
        private void OnCardSelected(Card selectedCard, List<Card> otherCards)
        {
            DeckEvents.OnCardSelected -= OnCardSelected;
            DeckServices.MoveToTable(selectedCard);
            DeckServices.Discard(otherCards);
            //DeckServices.DrawBackToFull();
        }
    }
}
