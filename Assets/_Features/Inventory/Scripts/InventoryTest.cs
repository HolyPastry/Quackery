using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery.Inventories
{
    public class InventoryTest : MonoBehaviour
    {
        public ItemData itemDataTest1;
        public ItemData itemDataTest2;

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            RunTestSequence();
        }

        private void RunTestSequence()
        {
            Debug.Log("Starting Inventory Test Sequence...");

            // Add items to inventory
            InventoryServices.AddNewItem(itemDataTest1);


            Assert.IsTrue(InventoryServices.HasItem(itemDataTest1),
                $"Item {itemDataTest1.name} should be in the inventory after adding.");

            //InventoryServices.RemoveItem(itemDataTest1);
            Assert.IsFalse(InventoryServices.HasItem(itemDataTest1),
                $"Item {itemDataTest1.name} should not be in the inventory after removing.");

            InventoryServices.AddNewItem(itemDataTest2);
            Assert.IsTrue(InventoryServices.HasItem(itemDataTest2),
                $"Item {itemDataTest2.name} should be in the inventory after adding.");

            var retrievedItem = InventoryServices.GetItem(itemDataTest2);

            Assert.IsNotNull(retrievedItem, $"Retrieved item should not be null for {itemDataTest2.name}.");
            Assert.AreEqual(itemDataTest2, retrievedItem.Data,
                $"Retrieved item data should match {itemDataTest2.name}.");

            Debug.Log("Inventory Test Sequence completed successfully.");

        }
    }
}