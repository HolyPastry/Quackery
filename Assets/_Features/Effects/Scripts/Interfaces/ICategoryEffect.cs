using Quackery.Inventories;


namespace Quackery.Effects
{

    public interface IValueEffect
    {
        public float Value { get; }
    }
    public interface ICategoryEffect
    {
        public EnumItemCategory Category { get; }
    }
}
