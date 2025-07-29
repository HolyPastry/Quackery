using System.Collections;
using Quackery.Notifications;
using Quackery.TetrisBill;
using UnityEngine;


namespace Quackery
{
    public class BankExpandedNotification : NotificationExpandedPanel
    {
        [SerializeField] private BlockPool _blockPool;

        public override IEnumerator Init(NotificationInfo info)
        {
            yield return base.Init(info);

            var bills = BillServices.GetAllBills();
            foreach (var bill in bills)
            {
                var shape = _blockPool.SpawnBlock(bill.Data.BlockPrefab);
                shape.SetLogo(bill.Data.Icon);
            }
        }
    }
}
