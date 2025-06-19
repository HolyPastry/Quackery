using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Bills
{
    public class BillOverdueUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _overdueBillUIs;
        private bool _initialized;

        private void OnEnable()
        {
            if (_initialized)
                UpdateUI();
        }

        IEnumerator Start()
        {
            yield return BillServices.WaitUntilReady();
            yield return CalendarServices.WaitUntilReady();

            UpdateUI();
            _initialized = true;
        }

        private void UpdateUI()
        {
            int numBills = BillServices.GetNumOverdueBills();
            for (int i = 0; i < _overdueBillUIs.Count; i++)
            {
                if (i < numBills)
                    _overdueBillUIs[i].SetActive(true);
                else
                    _overdueBillUIs[i].SetActive(false);
            }
        }
    }
}