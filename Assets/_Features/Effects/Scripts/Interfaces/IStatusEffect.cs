namespace Quackery.Effects
{
    public interface IStatusEffect
    {
        public Status Status { get; }
        public EnumEffectTrigger Trigger { get; }
    }
}
