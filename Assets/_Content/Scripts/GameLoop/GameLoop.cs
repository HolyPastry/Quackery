



using System;
using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using Quackery.Shops;
using Quackery.StateMachines;
using UnityEngine;


namespace Quackery
{
    public class GameLoop : StateMachine
    {
        [SerializeField] private float _delayDuration = 1f;
        public CardGameController CardGameController;
        //   public ClientListUI ClientListUI;
        //  public ClientChatRotatingPanels ClientTextChat;

        public ShopApp ShopApp;
        public App ChatApp;

        public BillApp BillApp;

        public App SleepApp;

        public App GameOverApp;

        public NotificationApp NotificationApp;

        private Client SelectedClient => ClientServices.SelectedClient();
        private bool ActiveClientSelected => SelectedClient != null
                && SelectedClient.IsInQueue && !SelectedClient.Served;

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return ClientServices.WaitUntilReady();
        }

        public void InitLoop()
        {
            var clientListState = new ClientListState(this);
            var cardGameState = new CardGameState(this);
            var passiveClientChatState = new PassiveClientChatState(this);
            var mainMenuState = new MainScreenState(this);
            var shopState = new ShopState(this);
            var billState = new BillState(this);
            var sleepState = new SleepState(this);
            var notificationState = new NotificationState(this);
            var gameOverState = new GameOverState(this);

            At(notificationState, clientListState, () => !NotificationApp.IsOpened);

            At(clientListState, cardGameState, () => ActiveClientSelected);
            At(cardGameState, billState, () => SelectedClient == null);
            At(shopState, sleepState, () => !ShopApp.IsOpened);

            At(billState, shopState, () => !BillApp.IsOpened && !BillApp.IsGameOver);
            At(billState, gameOverState, () => !BillApp.IsOpened && BillApp.IsGameOver);
            At(sleepState, notificationState, () => !SleepApp.IsOpened);

            ChangeState(notificationState);

        }

        internal void Delayed(Action hide)
        {
            StartCoroutine(DelayedCoroutine(hide));
        }

        private IEnumerator DelayedCoroutine(Action hide)
        {
            yield return new WaitForSeconds(_delayDuration);
            hide?.Invoke();
        }

    }
}
