using System.Collections;
using Quackery.StateMachines;

namespace Quackery
{
    public class PlayCardState : State
    {
        public PlayCardState(StateMachine stateMachine) : base(stateMachine)
        {
        }

        public override IEnumerator StateRoutine()
        {
            // Logic for playing a card
            yield return null;
        }
    }
}