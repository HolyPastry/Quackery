using KBCore.Refs;
using UnityEngine;


namespace Quackery
{
    public class BillAmountLiveUpdate : LiveTextUpdate
    {
        int _lastValue = -1;

        protected override WaitUntil WaitUntilReady => BillServices.WaitUntilReady();
        protected override void UpdateUI()
        {
            // int amountDue = BillServices.GetAmountDueToday();
            ///  if (amountDue == _lastValue) return;

            //  Text = $"{BillServices.GetAmountDueToday()}";
            if (_lastValue >= 0) PlayAudio();
            //   _lastValue = amountDue;

        }
    }
}
