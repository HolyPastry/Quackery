using Quackery.Decks;

namespace Quackery.Effects
{
    public interface IEffectRequirement
    {
        public bool IsFulfilled(Effect effect, Card card);

    }
}
