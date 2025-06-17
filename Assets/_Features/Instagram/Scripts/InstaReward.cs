using System.Collections;
using Holypastry.Bakery;
using UnityEngine;

namespace Quackery
{
    public abstract class InstaReward : ContentTag
    {
        public Sprite Banner;
        public string Description;
        public abstract void GiveReward();
    }
}
