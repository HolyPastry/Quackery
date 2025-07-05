using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;

using UnityEngine;

namespace Quackery.Inventories
{
    public class SceneSetupInventory : SceneSetupScript
    {
        [SerializeField] private List<ItemData> _itemDataList = new();

        [SerializeField] private bool _addAllItems = true;

        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return InventoryServices.WaitUntilReady();
            if (_addAllItems)
            {
                var dataCollection = new DataCollection<ItemData>("CardItems");
                foreach (var itemData in dataCollection.Data)
                {
                    InventoryServices.AddNewItem(itemData);
                }
            }
            foreach (var itemData in _itemDataList)
            {
                InventoryServices.AddNewItem(itemData);
            }

        }
    }
}