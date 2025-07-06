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

            DeckServices.Shuffle();

            // DialogQueueServices.QueueDialog("MeIntro");
            // yield return DialogQueueServices.WaitUntilAllDialogEnds();

            //bool firstTime = true;
            while (true)
            {
                var client = ClientServices.SelectedClient();
                if (client.IsAnonymous)
                {
                    textChat.Show(CustomerPanelSize.Short);
                    DialogQueueServices.QueueDialog("MeIntro");
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();

                }
                else
                {
                    textChat.Show(CustomerPanelSize.Special);
                    yield return new WaitForSeconds(0.5f);
                    DialogQueueServices.QueueDialog($"{client.DialogName}Quest");
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();
                }
                yield return new WaitForSeconds(0.5f);
                controller.StartNewRound(client);

                yield return controller.WaitUntilEndOfRound();

                DeckServices.DiscardHand();
                if (controller.RoundInterrupted)
                {
                    CartServices.SetCartValue(0);
                    client.BadReview();
                    ClientServices.ClientServed(client, false);

                }
                else
                {
                    DialogQueueServices.QueueDialog("MeConclusion");
                    DialogQueueServices.QueueDialog($"{client.DialogName}Success");
                    controller.TransfertCartToPurse();
                    yield return new WaitForSeconds(1f);
                    client.GoodReview();
                    ClientServices.ClientServed(client, true);
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();
                }


                // controller.ResetDeck();
                CartServices.DiscardCart();
                EffectServices.CleanEffects();

                controller.ShowEndRoundScreen(!controller.RoundInterrupted, out bool wasBoss);
                yield return controller.WaitUntilEndOfRoundScreenClosed();
                controller.HideEndRoundScreen();
                yield return new WaitForSeconds(1f);

                if (!ClientServices.HasNextClient() || wasBoss)
                {
                    controller.ShowEndDayScreen();
                    yield return controller.WaitUntilEndOfDayValidated();
                    ClientServices.ClientLeaves();
                    break;
                }
                else
                {
                    ClientServices.GetNextClient();
                    yield return textChat.WaitUntilClientCameIn();
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