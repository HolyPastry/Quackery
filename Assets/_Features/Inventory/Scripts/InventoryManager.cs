using System;
using System.Collections;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;
using UnityEngine;

public class InventoryManager : Service
{
    private const string SaveKey = "Inventory";
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
        _itemDataCollection = new("Items");
    }


    void OnDisable()
    {
        InventoryServices.WaitUntilReady = () => new WaitUntil(() => true);
        InventoryServices.AddItem = delegate { };
        InventoryServices.RemoveItem = delegate { };
        InventoryServices.GetItem = data => null;
        InventoryServices.HasItem = data => false;
    }

    void OnEnable()
    {
        InventoryServices.WaitUntilReady = () => WaitUntilReady;
        InventoryServices.AddItem = AddItem;
        InventoryServices.RemoveItem = RemoveItem;
        InventoryServices.GetItem = GetItem;
        InventoryServices.HasItem = HasItem;
    }

    protected override IEnumerator Start()
    {
        yield return FlowServices.WaitUntilReady();

        _inventory = SaveServices.Load<SerialInventory>(SaveKey);
        _inventory ??= new SerialInventory();
        foreach (var item in _inventory.Items)
        {
            item.Data = _itemDataCollection.GetFromName(item.Key);
        }
        _isReady = true;
    }

    private void AddItem(ItemData data, int arg2)
    {
        if (data == null)
        {
            Debug.LogWarning("Cannot add null item data to inventory.");
            return;
        }

        var item = GetItem(data);
        if (item != null)
        {
            item.Quantity += arg2;
        }
        else
        {
            item = new Item(data, arg2);
            _inventory.Items.Add(item);
        }

        Save();
    }

    private void RemoveItem(ItemData data, int arg2)
    {
        if (data == null)
        {
            Debug.LogWarning("Cannot remove null item data from inventory.");
            return;
        }

        var item = GetItem(data);
        if (item != null)
        {
            item.Quantity -= arg2;
            if (item.Quantity <= 0)
            {
                _inventory.Items.Remove(item);
            }
            Save();
        }
        else
        {
            Debug.LogWarning($"Item {data.MasterText} not found in inventory.");
        }
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
