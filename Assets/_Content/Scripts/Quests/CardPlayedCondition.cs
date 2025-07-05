
using Holypastry.Bakery.Quests;
using Quackery.GameStats;
using UnityEngine;

namespace Quackery
{

    [CreateAssetMenu(fileName = "NumCardPlayedCondition", menuName = "Quackery/Quests/Conditions/CardPlayedCondition")]
    public class CardPlayedCondition : Condition
    {

        public int NumCardRequired;
        public CardStats RequiredStats;

        public override string ToString()
        {
            int numCards = GameStatsServices.NumMatchingCard(RequiredStats);
            return $"{numCards}/{NumCardRequired} cards sold at reduced price";
        }

        public override bool Check
        {
            get
            {
                return GameStatsServices.NumMatchingCard(RequiredStats) >= NumCardRequired;
            }
        }

    }
}
