namespace Quackery
{
    internal class SleepState : GameState
    {
        public SleepState(GameLoop stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _gameLoop.SleepApp.Open();
        }

        public override void Exit()
        {
            CalendarServices.GoToNextDay();
        }

    }
}