using KBCore.Refs;
using UnityEngine;


namespace Quackery
{
    public class AmountDueGUIUpdate : LiveTextUpdate
    {
        protected override WaitUntil WaitUntilReady => BillServices.WaitUntilReady();
        protected override void UpdateUI()
        {
            Text = $"#Bill {BillServices.GetAmountDueToday()}";
        }
    }
}
