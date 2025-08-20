using System;
using Quackery.Decks;


namespace Quackery.Effects
{
    public interface IPriceModifierEffect : IValueEffect
    {
        public int PriceModifier(Effect effect, Card card) => 0;
        public float PriceMultiplier(Effect effect, Card card) => 1;

    }
}
