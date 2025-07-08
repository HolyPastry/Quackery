
using Holypastry.Bakery.Quests;
using UnityEngine;

namespace Quackery
{


    [CreateAssetMenu(fileName = "DialogLineResult", menuName = "Quackery/Quests/Play Dialog Line")]
    public class DialogLineResult : Result
    {
        [SerializeField] private string _dialogKnot;
        public override void Execute()
        {
            DialogQueueServices.QueueDialog(_dialogKnot);
        }
    }
}
