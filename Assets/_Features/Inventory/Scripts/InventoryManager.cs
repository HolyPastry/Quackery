using System;
using System.Collections;
using System.Collections.Generic;
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
            InventoryServices.AddItem = item => { };
            InventoryServices.RemoveItem = delegate { };
            InventoryServices.GetItem = data => null;
            InventoryServices.HasItem = data => false;
            InventoryServices.GetAllItems = () => new();
        }

        void OnEnable()
        {
            InventoryServices.WaitUntilReady = () => WaitUntilReady;
            InventoryServices.AddNewItem = AddItem;
            InventoryServices.AddItem = AddItem;
            InventoryServices.RemoveItem = RemoveItem;
            InventoryServices.GetItem = GetItem;
            InventoryServices.HasItem = HasItem;
            InventoryServices.GetAllItems = () => _inventory.Items;
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

            var item = new Item(data)
            {
                Price = data.StartPrice,
                Rating = data.StartRating
            };

            _inventory.Items.Add(new Item(data));



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