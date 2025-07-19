using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "CleanseEffect", menuName = "Quackery/Effects/Deck/Cleanse", order = 1)]
    public class CleanseEffect : EffectData
    {
        private Card _cardSelected;
        private List<Card> _otherCards;

        public override IEnumerator Execute(Effect effect)
        {
            _cardSelected = null;
            _otherCards = null;
            yield return true;
            // DeckServices.InterruptDraw();
            // List<Card> drawnCards = DeckServices.Draw(effect.Value);
            // DeckServices.MoveToCardSelect(drawnCards);
            // DeckEvents.OnCardSelected += OnCardSelected;

            // yield return new WaitUntil(() => _cardSelected != null);

            // DeckEvents.OnCardSelected -= OnCardSelected;
            // DeckServices.DestroyCard(_cardSelected);
            // DeckServices.Discard(_otherCards);
        }

        private void OnCardSelected(Card selectedCard, List<Card> otherCards)
        {
            _cardSelected = selectedCard;
            _otherCards = otherCards;

        }
    }
}
