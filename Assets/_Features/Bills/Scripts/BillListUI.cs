using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Bills;
using UnityEngine;

namespace Quackery
{
    public class BillListUI : MonoBehaviour
    {
        [SerializeField] private BillUI _billUIPrefab;
        [SerializeField] private Transform _billListContainer;
        private bool _initialized;

        void OnEnable()
        {
            if (_initialized) UpdateBillList();
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return BillServices.WaitUntilReady();
            UpdateBillList();
            _initialized = true;

        }

        private void UpdateBillList()
        {
            ClearList();
            // var bills = BillServices.GetAllBills();
            // foreach (var bill in bills)
            // {
            //     var billUI = Instantiate(_billUIPrefab, _billListContainer);
            //     billUI.Initialize(bill);
            // }
        }

        private void ClearList()
        {
            while (_billListContainer.childCount > 0)
            {
                var child = _billListContainer.GetChild(0);
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
                child.SetParent(null);
            }
        }
    }
}
