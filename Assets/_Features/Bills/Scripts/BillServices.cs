using System;
using System.Collections.Generic;
using Quackery.Bills;
using UnityEngine;


namespace Quackery
{

    public static class BillEvents
    {
        public static Action<Bill> OnBillUpdated = delegate { };
    }
    public static class BillServices
    {
        public static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);

        public static Func<List<Bill>> GetAllBills = () => new();

        internal static Action<BillData, bool> AddNewBill = delegate { };

        //  internal static Action<Bill> PayBill = delegate { };

        // internal static Func<Bill, int> DueIn = (bill) => 0;

        internal static Func<int> GetNumOverdueBills = () => 0;

        // internal static Func<int> GetAmountDueToday = () => 50;

        internal static Func<int> GetNumBillDueToday = () => 0;

        internal static Action ResetBills = delegate { };

        internal static Action<int> SetNumOverdueBills = (num) => { };
    }
}
