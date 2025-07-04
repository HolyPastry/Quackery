
using System;
using Holypastry.Bakery.Quests;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{

    [CreateAssetMenu(fileName = "CartValueCondition", menuName = "Quackery/Achievements/Cart Value Condition")]
    public class CartValueCondition : Condition
    {
        [SerializeField] private int _cartValueThreshold = 100;
        public override bool Check => CartServices.CanCartAfford(_cartValueThreshold);
    }
}
