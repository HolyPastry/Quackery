using Quackery.StateMachines;

namespace Quackery
{
    public class GameState : State
    {
        protected GameLoop _gameLoop;
        public GameState(GameLoop stateMachine) : base(stateMachine)
        {
            _gameLoop = stateMachine;
        }
    }
}