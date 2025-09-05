using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class QuotaUI : MonoBehaviour
    {
        [SerializeField] private int _quota;

        [SerializeField] private TextMeshProUGUI _quotaGUI;

        public int Quota => _quota;

        void Start()
        {
            _quotaGUI.text = _quota.ToString();
        }
    }
}
