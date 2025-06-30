


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
        public CardGameController CardGameController;
        //   public ClientListUI ClientListUI;
        public ClientChatRotatingPanels ClientTextChat;

        public ShopApp ShopApp;
        public App ChatApp;

        public BillApp BillApp;

        public App SleepApp;

        public App GameOverApp;

        public NotificationApp NotificationApp;

        private Client SelectedClient => ClientServices.SelectedClient();
        private bool ActiveClientSelected => SelectedClient != null
                && SelectedClient.IsInQueue && !SelectedClient.Served;
        private bool PassiveClientSelected =>
                SelectedClient != null &&
                (!SelectedClient.IsInQueue || SelectedClient.Served);



        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return ClientServices.WaitUntilReady();
            InitLoop();
        }

        private void InitLoop()
        {
            var clientListState = new ClientListState(this);
            var activeClientChatState = new ActiveClientChatState(this);
            var passiveClientChatState = new PassiveClientChatState(this);
            var mainMenuState = new MainScreenState(this);
            var shopState = new ShopState(this);
            var billState = new BillState(this);
            var sleepState = new SleepState(this);
            var notificationState = new NotificationState(this);
            var gameOverState = new GameOverState(this);

            At(notificationState, clientListState, () => !NotificationApp.IsOn);

            At(clientListState, activeClientChatState, () => ActiveClientSelected);
            At(activeClientChatState, shopState, () => SelectedClient == null);
            At(shopState, billState, () => !ShopApp.IsOn);

            At(billState, sleepState, () => !BillApp.IsOn && !BillApp.IsGameOver);
            At(billState, gameOverState, () => !BillApp.IsOn && BillApp.IsGameOver);
            At(sleepState, notificationState, () => !SleepApp.IsOn);

            At(clientListState, passiveClientChatState, () => PassiveClientSelected);
            At(passiveClientChatState, clientListState, () => SelectedClient == null);

            ChangeState(notificationState);

        }
    }
}