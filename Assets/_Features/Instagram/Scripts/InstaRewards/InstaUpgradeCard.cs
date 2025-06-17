using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "InstaUpgradeCard", menuName = "Quackery/Instagram/InstaUpgradeCard")]
    public class InstaUpgradeCard : InstaReward
    {
        public override void GiveReward()
        {
            // This is a placeholder for the upgrade logic.
            // The actual implementation would depend on how cards are upgraded in the game.
            // For example, it could involve increasing the card's stats or abilities.
            Debug.Log("Card upgrade logic not implemented yet.");
        }
    }
}
