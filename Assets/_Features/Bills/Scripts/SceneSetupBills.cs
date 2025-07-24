using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using Quackery.Bills;


namespace Quackery
{
    public class SceneSetupBills : SceneSetupScript
    {
        public List<BillData> BillDataList;
        public int NumOverdueBills = 0;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return BillServices.WaitUntilReady();
            foreach (var billData in BillDataList)
            {
                BillServices.AddNewBill(billData, false);
            }
            BillServices.SetNumOverdueBills(NumOverdueBills);
        }
    }
}
