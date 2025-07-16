using System.Collections;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "DuplicateAndDiscardEffect", menuName = "Quackery/Effects/Deck/Duplicate And Discard")]
    public class DuplicateAndDiscardEffect : EffectData
    {
        public enum CardPosition
        {
            TopDraw,
            TopDiscard,
            TopCart
        }

        [SerializeField] private CardPosition _position = CardPosition.TopDraw;

        public override IEnumerator Execute(Effect effect)
        {
            Card card = null;
            switch (_position)
            {
                case CardPosition.TopDraw:
                    card = DeckServices.GetTopCard(EnumCardPile.Draw);
                    break;
                case CardPosition.TopDiscard:
                    card = DeckServices.GetTopCard(EnumCardPile.Discard);
                    break;
                case CardPosition.TopCart:
                    card = DeckServices.GetTopCard(EnumCardPile.Cart);
                    break;
            }
            if (card == null) yield break;

            Card duplicate = DeckServices.DuplicateCard(card);
            yield return DefaultWaitTime;
            DeckServices.Discard(new() { duplicate });
            yield return DefaultWaitTime;


        }
    }
}
