using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public class BillTest : MonoBehaviour
    {
        [SerializeField] private App _billApp;
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();


            _billApp.Show();
        }


    }
}
