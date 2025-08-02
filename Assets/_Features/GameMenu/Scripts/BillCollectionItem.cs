using System;
using Quackery.Bills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    internal class BillCollectionItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _billName;
        [SerializeField] private Transform _shapeParent;
        internal void Initialize(Bill bill)
        {
            _billName.text = bill.Data.MasterText;
            Instantiate(bill.Data.BlockPrefab, _shapeParent);
        }
    }
}

