using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "CleanseEffect", menuName = "Quackery/Effects/Cleanse", order = 1)]
    public class CleanseEffect : EffectData
    {
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
            DeckServices.DestroyCard(selectedCard);
            DeckServices.Discard(otherCards);
            //DeckServices.DrawBackToFull();
        }
    }
}
