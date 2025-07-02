using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public class PurseTextUpdate : LiveTextUpdate
    {

        protected override WaitUntil WaitUntilReady => PurseServices.WaitUntilReady();
        protected override void UpdateUI()
        {
            Text = $"#Coin {PurseServices.GetString()}";
        }


    }
}
