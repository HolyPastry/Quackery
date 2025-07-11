using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "PriceModifierBasedOnPriceEffect", menuName = "Quackery/Effects/Price/Price Modifier Based On Price", order = 0)]
    public class PriceModifierBasedOnPriceEffect : EffectData
    {
        [SerializeField] private int _priceToMatch = 1;

        public override void Execute(Effect effect)
        {

            DeckServices.BoostPriceOfCardsInHand(effect.Value, Card => Card.Price == _priceToMatch);
        }
    }
}