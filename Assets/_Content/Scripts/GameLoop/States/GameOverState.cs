using Quackery.StateMachines;

namespace Quackery
{
    internal class GameOverState : GameState
    {
        public GameOverState(GameLoop stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _gameLoop.GameOverApp.Open();
        }
    }
}