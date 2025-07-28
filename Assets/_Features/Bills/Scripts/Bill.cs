using System;

namespace Quackery.Bills
{
    public record Bill
    {
        public BillData Data;
        public string Key;
        // public int Price => Data.Price;

        // public int TotalPrice => Price * (NumMissedPayments + 1);
        public bool Paid;
        public int LastPaymentDay;

        public int StartedData;

        public string Title => Data.MasterText;

        public int NumMissedPayments = 0;
        public bool IsOverdue => NumMissedPayments > 0;

        public bool New = false;

        public Bill()
        { }

        public Bill(BillData data)
        {
            Data = data;
            Key = Data.name;
            //Price = data.Price;
            Paid = false;
        }

        internal bool IsDueToday()
        {
            return true;
            // var daysPassed = CalendarServices.Today() - StartedData;
            // var daysSinceLastDueDate = daysPassed % Data.PaymentInterval;
            // var lastDueDate = daysPassed - daysSinceLastDueDate + 1;
            // return lastDueDate == CalendarServices.Today();
        }
    }
}
