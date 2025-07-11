namespace Quackery
{
    internal class SleepState : GameState
    {
        public SleepState(GameLoop stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _gameLoop.SleepApp.Show();
        }

        public override void Exit()
        {
            CalendarServices.GoToNextDay();
        }

    }
}