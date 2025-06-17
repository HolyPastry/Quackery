using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "InstaRemoveCard", menuName = "Quackery/Instagram/InstaRemoveCard")]
    public class InstaRemoveCard : InstaReward
    {
        public override void GiveReward()
        {
            DeckServices.RemoveCardRequest();
        }
    }
}
