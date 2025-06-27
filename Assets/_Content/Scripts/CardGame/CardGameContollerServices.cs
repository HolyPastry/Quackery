using System;



namespace Quackery.Decks
{
    public static class CardGameContollerServices
    {
        public static Func<int, bool> CanCartAfford = (value) => true;
        public static Action<int> ModifyCartCash = (amount) => { };
    }
}
