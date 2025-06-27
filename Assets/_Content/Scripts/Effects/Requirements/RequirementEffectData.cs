using Quackery.Decks;

namespace Quackery.Effects
{
    public class RequirementEffectData : EffectData
    {
        public virtual bool IsFulfilled(Effect effect, Card card)
        {
            return true;
        }
    }
}
