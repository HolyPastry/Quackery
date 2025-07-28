using System;
using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;

public static class InventoryServices
{
    public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);
    public static Func<ItemData, Item> AddNewItem = (itemData) => null;
    public static Action<Item> AddItem = item => { };
    public static Action<Item> RemoveItem = delegate { };
    public static Func<ItemData, Item> GetItem = data => null;
    public static Func<ItemData, bool> HasItem = data => false;

    internal static Func<List<Item>> GetAllItems = () => new();

    internal static Func<ItemData> GetRandomItemData = () => null;

    internal static Func<int, List<ItemData>> GetRandomItems = (amount) => new();

    internal static Action<List<ItemData>> AddNewItems = delegate { };

    internal static Func<Predicate<ItemData>, ItemData> GetRandomMatchingItem = (predicate) => null;

}
