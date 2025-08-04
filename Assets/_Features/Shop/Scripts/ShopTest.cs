using System.Collections;
using System.Collections.Generic;
using Quackery.GameMenu;
using UnityEngine;

namespace Quackery.Shops
{
    public class ShopTest : MonoBehaviour
    {
        [SerializeField] private ShopApp _instagramUI;
        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            GameMenuController.ShowRequest();
            _instagramUI.Open();
        }
    }
}

