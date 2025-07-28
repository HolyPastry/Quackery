using System;
using System.Linq;
using UnityEngine;


namespace Quackery.Bills
{
    [CreateAssetMenu(fileName = "SubscriptionBillData", menuName = "Quackery/Bills/SubscriptionBillData", order = 1)]
    public class SubscriptionBillData : BillData
    {
        //   public override int Price => CalculateSubscriptionPrice();

        private int CalculateSubscriptionPrice()
        {
            var allItems = InventoryServices.GetAllItems();
            return allItems.Sum(item => item.Data.SubscriptionCost);
        }
    }
}
