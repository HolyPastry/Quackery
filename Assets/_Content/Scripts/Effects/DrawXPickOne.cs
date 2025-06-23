using System.Collections.Generic;
using Quackery.Decks;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "SeerEffect", menuName = "Quackery/Effects/Seer", order = 1)]
    public class DrawXPickOne : EffectData
    {
        // public override string Description => "Draw two cards from the deck, select one to keep, and discard the other.";

        public override void Execute(Effect effect, CardPile drawPile)
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
            DeckServices.DrawBackToFull();
        }
    }
}
