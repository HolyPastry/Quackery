using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class ActiveClientChatState : GameState
    {
        public ActiveClientChatState(GameLoop gameLoop) : base(gameLoop)
        { }

        public override IEnumerator StateRoutine()
        {
            var controller = _gameLoop.CardGameController;
            var textChat = _gameLoop.ClientTextChat;

            controller.Show();
            textChat.Show(CustomerPanelSize.Short);
            DeckServices.Shuffle();

            // DialogQueueServices.QueueDialog("MeIntro");
            // yield return DialogQueueServices.WaitUntilAllDialogEnds();

            bool firstTime = true;
            while (true)
            {
                var client = ClientServices.SelectedClient();
                // if (!firstTime)
                // {
                DialogQueueServices.QueueDialog("MeIntro");
                yield return DialogQueueServices.WaitUntilAllDialogEnds();
                // }

                // firstTime = false;
                controller.StartNewRound(client);

                yield return controller.WaitUntilEndOfRound();

                DeckServices.DiscardHand();
                if (controller.RoundInterrupted)
                {
                    CartServices.SetCartValue(0);
                    client.BadReview();
                    ClientServices.ClientServed(client.Data);

                }
                else
                {
                    DialogQueueServices.QueueDialog("MeConclusion");
                    DialogQueueServices.QueueDialog($"{client.DialogKey}Success");
                    controller.TransfertCartToPurse();
                    yield return new WaitForSeconds(1f);
                    client.GoodReview();
                    ClientServices.ClientServed(client.Data);
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();
                }


                // controller.ResetDeck();
                CartServices.DiscardCart();
                EffectServices.CleanEffects();

                controller.ShowEndRoundScreen(!controller.RoundInterrupted);
                yield return controller.WaitUntilEndOfRoundScreenClosed();
                controller.HideEndRoundScreen();
                yield return new WaitForSeconds(1f);


                if (ClientServices.HasNextClient())
                {
                    ClientServices.GetNextClient();
                    yield return textChat.WaitUntilClientCameIn();
                }
                else
                {
                    controller.ShowEndDayScreen();
                    yield return controller.WaitUntilEndOfDayValidated();
                    ClientServices.ClientLeaves();
                    break;
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            _gameLoop.CardGameController.Hide();
            //  _gameLoop.ClientTextChat.Hide();
            _gameLoop.ChatApp.Hide();
        }
    }
}