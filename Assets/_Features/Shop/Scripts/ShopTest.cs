using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Shops
{
    public class ShopTest : MonoBehaviour
    {
        [SerializeField] private ShopApp _instagramUI;
        void Start()
        {
            _instagramUI.Show();
        }
    }
}

