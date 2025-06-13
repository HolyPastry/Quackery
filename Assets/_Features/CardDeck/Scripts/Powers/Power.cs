using Holypastry.Bakery;

namespace Quackery.Decks
{
    public abstract class Power : ContentTag
    {
        public EnumPowerTrigger Trigger;
        public abstract void Execute(CardPile pile);

        public abstract string Description { get; }

    }
}
