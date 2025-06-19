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
        private const string BillCollectionKey = "Bills";
        private DataCollection<BillData> _billDataCollection;
        private readonly List<Bill> _bills;
        public List<Bill> Bills => _bills;

        public BillList()
        {
            _billDataCollection = new DataCollection<BillData>(BillCollectionKey);
            var bills = SaveServices.Load<BillList>(BillCollectionKey);
            if (bills != null)
                _bills = bills._bills;
            else
                _bills = new List<Bill>();
        }

        public Bill AddNewBill(BillData billData)
        {
            var bill = new Bill(billData);

            _bills.Add(bill);
            Save();
            return bill;
        }

        private void Save()
        {
            SaveServices.Save(BillCollectionKey, this);
        }

        public void RemoveBill(BillData billData)
        {
            _bills.RemoveAll(b => b.Data == billData);
            Save();
        }

        public override void Serialize()
        {
            base.Serialize();

        }
        public override void Deserialize()
        {
            base.Deserialize();
            foreach (var bill in _bills)
            {
                bill.Data = _billDataCollection.GetFromName(bill.Key);
                Assert.IsNotNull(bill.Data, $"Bill data for {bill.Key} not found in collection");
            }
        }

        internal void PayBill(Bill bill)
        {
            Assert.IsTrue(_bills.Contains(bill), "Bill not found in the list");
            bill.Paid = true;
            bill.LastPaymentDay = CalendarServices.Today();
            Save();
        }

        internal int GetNumOverdueBills()
        {
            int overdueCount = _bills.Sum(bill => bill.IsOverdue ? 1 : 0);

            return overdueCount;
        }
    }
}
