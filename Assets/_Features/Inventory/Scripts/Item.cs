using System;

namespace Quackery.Inventories
{
    [Serializable]
    public class Item
    {
        [NonSerialized]
        public ItemData Data;
        public string Key;
        public int Quantity;

        public Item(ItemData data, int quantity)
        {
            Data = data;
            Quantity = quantity;
            Key = Data.name;
        }

        public string Name => Data.MasterText;
    }
}