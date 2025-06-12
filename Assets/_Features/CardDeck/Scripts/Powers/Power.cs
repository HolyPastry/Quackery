using Holypastry.Bakery;

namespace Quackery.Decks
{
    public enum EnumPowerTrigger
    {
        OnCardMoveToCart,
        OnCardDrawn,
        OnCardDiscarded,
    }
    public abstract class Power : ContentTag
    {
        public EnumPowerTrigger Trigger;
        public abstract void Execute(CardPile pile);

        public abstract string Description { get; }

    }
}
