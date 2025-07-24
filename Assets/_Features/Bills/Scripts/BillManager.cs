using System;
using System.Collections;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery.Flow;

using UnityEngine;


namespace Quackery.Bills
{

    public class BillManager : Service
    {
        private BillList _billList;


        void OnEnable()
        {
            BillServices.WaitUntilReady = () => WaitUntilReady;
            BillServices.GetAllBills = () => _billList.Bills.FindAll(b => b.Price > 0);
            BillServices.AddNewBill = AddNewBill;
            BillServices.PayBill = PayBill;
            BillServices.DueIn = CalculateDueInDate;
            BillServices.GetNumOverdueBills = () => _billList.GetNumOverdueBills();
            BillServices.GetAmountDueToday = GetAmountDueToday;
            BillServices.GetNumBillDueToday = GetNumBillDueToday;
            BillServices.ResetBills = ResetBills;

        }



        void OnDisable()
        {
            BillServices.WaitUntilReady = () => new WaitUntil(() => true);
            BillServices.GetAllBills = () => new List<Bill>();
            BillServices.AddNewBill = delegate { };
            BillServices.PayBill = delegate { };
            BillServices.DueIn = (bill) => 0;
            BillServices.GetNumOverdueBills = () => 0;
            BillServices.GetAmountDueToday = () => 0;
            BillServices.GetNumBillDueToday = () => 0;
            BillServices.ResetBills = delegate { };
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _billList = SaveServices.Load<BillList>(BillList.BillCollectionKey);
            _billList ??= new();
            _isReady = true;
        }

        private void ResetBills()
        {
            _billList.ResetPaidStatus();
        }

        private void AddNewBill(BillData data, bool IsNew)
        {
            var bill = _billList.AddNewBill(data);
            bill.StartedData = CalendarServices.Today();
            bill.New = IsNew;
            BillEvents.OnBillUpdated(bill);
        }

        private int GetNumBillDueToday() => _billList.GetNumBillDueToday();

        private int CalculateDueInDate(Bill bill)
        {
            var daysPassed = CalendarServices.Today() - bill.StartedData;
            var daysSinceLastDueDate = daysPassed % bill.Data.PaymentInterval;
            var lastDueDate = daysPassed - daysSinceLastDueDate;
            if (bill.LastPaymentDay < lastDueDate)
            {
                bill.NumMissedPayments = (lastDueDate - bill.LastPaymentDay) / bill.Data.PaymentInterval;
                BillEvents.OnBillUpdated(bill);
                return lastDueDate;
            }
            else
            {

                bill.NumMissedPayments = 0;
                BillEvents.OnBillUpdated(bill);
                return lastDueDate + bill.Data.PaymentInterval;
            }

        }

        private void PayBill(Bill bill)
        {
            _billList.PayBill(bill);
            PurseServices.Modify(-bill.TotalPrice);
            BillEvents.OnBillUpdated(bill);

        }

        private int GetAmountDueToday()
        {
            int total = 0;
            foreach (var bill in _billList.Bills)
            {
                if (bill.IsDueToday())
                {
                    total += bill.TotalPrice;
                }
            }
            return total;
        }

    }
}