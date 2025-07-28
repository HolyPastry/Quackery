using Holypastry.Bakery.Quests;
using UnityEngine;

namespace Quackery.Progression
{
    [CreateAssetMenu(fileName = "LevelCondition", menuName = "Quackery/Quests/Conditions/Level", order = 1)]
    public class ProgressionCondition : Condition
    {
        [SerializeField] private int _level;
        public override bool Check => ProgressionServices.GetLevel() >= _level;
    }
}
