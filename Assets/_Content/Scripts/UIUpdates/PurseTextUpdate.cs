using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Quackery
{
    public class PurseTextUpdate : LiveTextUpdate
    {
        private int _lastPurseValue = -1;
        protected override WaitUntil WaitUntilReady => PurseServices.WaitUntilReady();
        protected override void UpdateUI()
        {
            int value = PurseServices.GetAmount();

            if (_lastPurseValue == value)
                return;

            Text = $"<sprite name=Money>{PurseServices.GetString()}";
            Punch();
            // if (value < _lastPurseValue)
            //     PlayAudio();
            _lastPurseValue = value;
        }


    }
}
