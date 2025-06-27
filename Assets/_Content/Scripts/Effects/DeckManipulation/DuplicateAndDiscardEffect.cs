using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "DuplicateAndDiscardEffect", menuName = "Quackery/Effects/Duplicate And Discard")]
    public class DuplicateAndDiscardEffect : EffectData
    {
        public enum CardPosition
        {
            TopDraw,
            TopDiscard,
            TopCart
        }

        [SerializeField] private CardPosition _position = CardPosition.TopDraw;

        public override void Execute(Effect effect, CardPile pile)
        {
            Card card = null;
            switch (_position)
            {
                case CardPosition.TopDraw:
                    card = DeckServices.GetTopCard(EnumPileType.DrawPile);
                    break;
                case CardPosition.TopDiscard:
                    card = DeckServices.GetTopCard(EnumPileType.DiscardPile);
                    break;
                case CardPosition.TopCart:
                    card = DeckServices.GetTopCard(EnumPileType.InCart);
                    break;
            }
            if (card == null) return;

            Card duplicate = DeckServices.DuplicateCard(card);
            DeckServices.Discard(new() { duplicate });


        }
    }
}
