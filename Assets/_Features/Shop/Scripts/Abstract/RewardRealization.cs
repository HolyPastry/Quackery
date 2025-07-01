using System;
using System.Collections;

using UnityEngine;

namespace Quackery.Shops
{
    public abstract class RewardRealization : MonoBehaviour
    {

        public Action OnRealizationComplete = delegate { };

        public abstract IEnumerator RealizationRoutine(ShopReward reward);
    }
}
