using Quackery.Decks;


namespace Quackery.Effects
{


    public interface IPriceModifierEffect
    {
        public int PriceModifier(Effect effect, Card card) => 0;
        public float PriceMultiplier(Effect effect, Card card) => 0;

    }
}
