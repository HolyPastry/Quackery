using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bakery.Saves;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;

using UnityEngine;

namespace Quackery.Inventories
{
    public class InventoryManager : Service
    {
        private const string SaveKey = "Inventory";
        [SerializeField] private string CollectionKey = "Items";
        [Serializable]
        public class SerialInventory : SerialData
        {
            public List<Item> Items = new();
        }


        private DataCollection<ItemData> _itemDataCollection;
        // Start is called before the first frame update
        private SerialInventory _inventory;

        void Awake()
        {
            _itemDataCollection = new(CollectionKey);
        }

        void OnDisable()
        {
            InventoryServices.WaitUntilReady = () => new WaitUntil(() => true);
            InventoryServices.AddNewItem = (data) => null;
            InventoryServices.AddNewItems = items => { };
            InventoryServices.AddItem = item => { };

            InventoryServices.RemoveItem = delegate { };
            InventoryServices.GetItem = data => null;
            InventoryServices.HasItem = data => false;
            InventoryServices.GetAllItems = () => new();
            InventoryServices.GetRandomItemData = () => null;
            InventoryServices.GetRandomItems = amount => new();
        }

        void OnEnable()
        {
            InventoryServices.WaitUntilReady = () => WaitUntilReady;
            InventoryServices.AddNewItem = AddItem;
            InventoryServices.AddNewItems = AddNewItems;
            InventoryServices.AddItem = AddItem;
            InventoryServices.RemoveItem = RemoveItem;
            InventoryServices.GetItem = GetItem;
            InventoryServices.HasItem = HasItem;
            InventoryServices.GetAllItems = () => _inventory.Items;
            InventoryServices.GetRandomItemData = GetRandomItemData;
            InventoryServices.GetRandomItems = GetRandomItems;
        }

        private void AddNewItems(List<ItemData> list)
        {

            if (list == null)
                _itemDataCollection.Data.ForEach(itemData => AddItem(itemData));
            else
                list.ForEach(itemData => AddItem(itemData));
        }

        private List<ItemData> GetRandomItems(int amount)
        {

            List<ItemData> randomItems = new();
            List<ItemData> itemDataList = _itemDataCollection.Data.ToList();
            itemDataList.Shuffle();
            for (int i = 0; i < Math.Min(amount, itemDataList.Count); i++)
                randomItems.Add(itemDataList[i]);

            return randomItems;
        }

        private ItemData GetRandomItemData()
        {

            int randomIndex = UnityEngine.Random.Range(0, _itemDataCollection.Count);
            return _itemDataCollection.Data[randomIndex];
        }

        private void AddItem(Item item)
        {
            if (item == null || item.Data == null)
            {
                Debug.LogWarning("Cannot add null item or item with null data to inventory.");
                return;
            }
            _inventory.Items.Add(item);
            Save();

        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();

            _inventory = SaveServices.Load<SerialInventory>(SaveKey);
            _inventory ??= new SerialInventory();
            int i = 0;
            while (i < _inventory.Items.Count)
            {
                var item = _inventory.Items[i];
                item.Data = _itemDataCollection.GetFromName(item.Key);
                if (item.Data == null)
                {
                    Debug.LogWarning($"Item data for {item.Key} not found in collection. Removing item from inventory.");
                    _inventory.Items.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            _isReady = true;
        }

        private Item AddItem(ItemData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Cannot add null item data to inventory.");
                return null;
            }

            data.CheckValidity();

            var item = new Item(data);

            _inventory.Items.Add(item);
            Save();
            return item;
        }

        private void RemoveItem(Item item)
        {
            _inventory.Items.Remove(item);
            Save();
        }

        private void Save()
        {
            foreach (var item in _inventory.Items)
            {
                item.Key = item.Data.name;

            }
            SaveServices.Save(SaveKey, _inventory);
        }

        private Item GetItem(ItemData data)
        {
            if (HasItem(data))
            {
                return _inventory.Items.Find(item => item.Data == data);
            }
            return null;
        }

        private bool HasItem(ItemData data)
        {
            return _inventory.Items.Exists(item => item.Data == data);
        }
    }
}