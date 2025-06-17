using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "InstaCardRewardData", menuName = "Quackery/Instagram/InstaCardRewardData")]
    public class InstaCardRewardData : InstaReward
    {
        public ItemData Card;

        public override void GiveReward()
        {
            DeckServices.AddToDeck(new() { Card });
        }
    }
}
