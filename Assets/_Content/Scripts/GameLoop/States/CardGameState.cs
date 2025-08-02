using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using Quackery.GameMenu;
using UnityEngine;

namespace Quackery
{
    public class CardGameState : GameState
    {
        private CardGameApp _cardGame;
        public CardGameState(GameLoop gameLoop) : base(gameLoop)
        {
            _cardGame = gameLoop.CardGameApp;
        }

        public CardGameState(CardGameApp gameApp) : base(null)
        {
            _cardGame = gameApp;
        }

        public override IEnumerator StateRoutine()
        {
            GameMenuController.HideRequest();
            _cardGame.Init();
            _cardGame.Open();

            while (true)
            {
                var client = ClientServices.GetNextClient();

                CartServices.ResetCart();
                _cardGame.StartNewRound(client);

                yield return _cardGame.WaitUntilEndOfRound();

                if (_cardGame.RoundInterrupted)
                {
                    CartServices.ResetCart();
                    client.BadReview();
                    ClientServices.ClientServed(client, false);
                }
                else
                {
                    DialogQueueServices.QueueDialog("MeConclusion");
                    DialogQueueServices.QueueDialog($"{client.DialogName}Success");

                    client.GoodReview();
                    ClientServices.ClientServed(client, true);
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();
                }

                _cardGame.ShowEndRoundScreen(out bool wasBoss);
                yield return _cardGame.WaitUntilEndOfRoundScreenClosed();
                _cardGame.HideEndRoundScreen();
                _cardGame.TransfertCartToPurse();

                EffectServices.CleanEffects();
                yield return new WaitForSeconds(1f);

                if (!ClientServices.HasNextClient() || wasBoss)
                {
                    DeckServices.ResetDecks();
                    ClientServices.ClientLeaves();
                    break;
                }
                else
                {
                    DeckServices.DiscardHand();
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            _cardGame.Close();
        }
    }
}