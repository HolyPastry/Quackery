using System.Collections.Generic;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "InstaCardBoosterData", menuName = "Quackery/Instagram/InstaCardBoosterData")]
    public class InstaCardBoosterData : InstaReward
    {
        public List<ItemData> Cards;

        public override void GiveReward()
        {
            int randomIndex = UnityEngine.Random.Range(0, Cards.Count);
            ItemData card = Cards[randomIndex];
            DeckServices.AddToDeck(new() { card });
        }
    }
}
