
using Holypastry.Bakery.Quests;
using Quackery.Narrative;
using UnityEngine;

namespace Quackery
{


    [CreateAssetMenu(fileName = "NarrativeResult", menuName = "Quackery/Quests/Narrative Result")]
    public class NarrativeResult : Result
    {
        [SerializeField] private NarrativeData _narrative;
        public override void Execute()
        {
            NarrativeServices.AddNarrative(_narrative);
        }
    }
}
