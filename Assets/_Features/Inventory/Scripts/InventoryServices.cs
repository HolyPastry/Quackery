using System;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;

public static class InventoryServices
{
    public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
    public static Action<ItemData, int> AddItem = delegate { };
    public static Action<ItemData, int> RemoveItem = delegate { };
    public static Func<ItemData, Item> GetItem = data => null;
    public static Func<ItemData, bool> HasItem = data => false;

    internal static Func<List<Item>> GetAllItems = () => new();

}
