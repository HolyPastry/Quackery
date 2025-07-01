using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;

using UnityEngine;

namespace Quackery.Inventories
{
    public class SceneSetupInventory : SceneSetupScript
    {
        [SerializeField] private List<ItemData> _itemDataList = new();

        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return InventoryServices.WaitUntilReady();

            foreach (var itemData in _itemDataList)
            {
                InventoryServices.AddNewItem(itemData);
            }

        }
    }
}