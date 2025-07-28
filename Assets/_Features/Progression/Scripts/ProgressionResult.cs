using Holypastry.Bakery.Quests;
using UnityEngine;

namespace Quackery.Progression
{
    [CreateAssetMenu(fileName = "LevelResult", menuName = "Quackery/Quests/Results/Level", order = 1)]
    public class ProgressionResult : Result
    {
        [SerializeField] private int _level;
        public override void Execute()
        {
            ProgressionServices.SetLevel(_level);
        }
    }
}
