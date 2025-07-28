using System;
using System.Collections.Generic;
using System.Linq;
using Bakery.Saves;
using Holypastry.Bakery;
using UnityEngine.Assertions;


namespace Quackery.Bills
{
    public class BillList : SerialData
    {
        public const string BillCollectionKey = "Bills";
        private DataCollection<BillData> _billDataCollection;

        public List<Bill> Bills = new();
        private int _overrideNumOverdueBill;

        public BillList()
        {
            _billDataCollection = new DataCollection<BillData>(BillCollectionKey);

        }

        public Bill AddNewBill(BillData billData)
        {
            var bill = new Bill(billData);

            Bills.Add(bill);
            Save();
            return bill;
        }

        private void Save()
        {
            SaveServices.Save(BillCollectionKey, this);
        }

        public void RemoveBill(BillData billData)
        {
            Bills.RemoveAll(b => b.Data == billData);
            Save();
        }

        public override void Serialize()
        {
            base.Serialize();

        }
        public override void Deserialize()
        {
            base.Deserialize();
            foreach (var bill in Bills)
            {
                bill.Data = _billDataCollection.GetFromName(bill.Key);
                Assert.IsNotNull(bill.Data, $"Bill data for {bill.Key} not found in collection");
            }
        }

        internal void PayBill(Bill bill)
        {
            Assert.IsTrue(Bills.Contains(bill), "Bill not found in the list");
            bill.Paid = true;
            bill.LastPaymentDay = CalendarServices.Today();
            Save();
        }

        internal int GetNumOverdueBills()
        {
            if (_overrideNumOverdueBill > 0)
            {
                return _overrideNumOverdueBill;
            }
            int overdueCount = Bills.Sum(bill => bill.IsOverdue ? 1 : 0);

            return overdueCount;
        }
        public int GetNumBillDueToday()
        {
            return 0;
            // return Bills.Sum(bill => (!bill.Paid &&
            //                     !bill.New &&
            //                     bill.Price > 0) ? 1 : 0);
        }

        internal void ResetPaidStatus()
        {
            foreach (var bill in Bills)
            {
                bill.Paid = false;
                bill.New = false;
                bill.NumMissedPayments = 0;
                bill.LastPaymentDay = CalendarServices.Today();
            }
            Save();
        }

        internal void OverrideNumOverdueBill(int num)
        {
            _overrideNumOverdueBill = num;
        }
    }
}
