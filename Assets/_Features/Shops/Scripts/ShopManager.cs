

using System;
using System.Collections;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
using UnityEngine;


namespace Quackery.Shops
{
    public class ShopManager : Service
    {
        [SerializeField] private string CollectionKey = "Items";
        [SerializeField] private string ShopCollectionKey = "Shops";

        [Serializable]
        public class SerialShop : SerialData
        {
            public string Key;
            public List<Item> Items = new();
        }

        private DataCollection<ItemData> _itemDataCollection;
        private DataCollection<ShopData> _shopDataCollection;
        private readonly Dictionary<ShopData, SerialShop> _shops = new();

        void Awake()
        {
            _itemDataCollection = new(CollectionKey);
            _shopDataCollection = new(ShopCollectionKey);
        }

        void OnDisable()
        {
            ShopServices.WaitUntilReady = () => new WaitUntil(() => true);
            ShopServices.GetAllItems = (shopData) => new();
        }

        void OnEnable()
        {
            ShopServices.WaitUntilReady = () => WaitUntilReady;
            ShopServices.GetAllItems =
                (shopData) => _shops.ContainsKey(shopData) ? _shops[shopData].Items : new();
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            LoadShops();

            _isReady = true;
        }

        private void LoadShops()
        {

            foreach (var shopData in _shopDataCollection.Data)
            {
                var serialShop = SaveServices.Load<SerialShop>(shopData.name);

                serialShop ??= new SerialShop { Key = shopData.name };

                int i = 0;
                while (i < serialShop.Items.Count)
                {
                    var item = serialShop.Items[i];
                    item.Data = _itemDataCollection.GetFromName(item.Key);
                    if (item.Data == null)
                    {
                        Debug.LogWarning($"Item data for {item.Key} not found in collection. Removing item from shop.");
                        serialShop.Items.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                _shops[shopData] = serialShop;
            }
        }
    }
}