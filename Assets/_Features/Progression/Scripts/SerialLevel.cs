using Bakery.Saves;

namespace Quackery.Progression
{
    public class SerialLevel : SerialData
    {
        public int CurrentLevel;

        public static implicit operator int(SerialLevel serial) => serial.CurrentLevel;
        public static implicit operator SerialLevel(int level) => new() { CurrentLevel = level };
    }
}
