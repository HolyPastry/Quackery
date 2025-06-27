using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using TMPro;
using UnityEngine;

namespace Quackery
{

    public class StackInfoUI : ValidatedMonoBehaviour
    {

        [SerializeField, Child] private TextMeshProUGUI _countText;

        public void SetCount(int herbCount)
        {
            gameObject.SetActive(herbCount > 0);
            _countText.text = herbCount.ToString();
        }
        public void ClearCount()
        {
            gameObject.SetActive(false);
            _countText.text = "0";
        }

    }
}
