using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.TetrisBill
{
    public class TetrisController : MonoBehaviour
    {
        [SerializeField] private TetrisGame _tetrisGame;
        [SerializeField] private TetrisStartScreen _startScreen;
        [SerializeField] private TetrisOverdueUI _overdueUI;
        [SerializeField] private TetrisEndScreen _endScreen;

        private bool _startedOnce;
        internal static Action ResetRequest = delegate { };
        void OnEnable()
        {
            _startScreen.OnStart += StartGame;
            _tetrisGame.OnGameOver += GameOver;
            ResetRequest = Reset;

            if (_startedOnce) Setup();

        }

        void OnDisable()
        {
            _startScreen.OnStart -= StartGame;
            _tetrisGame.OnGameOver -= GameOver;
            ResetRequest = delegate { };
        }

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return BillServices.WaitUntilReady();
            yield return PurseServices.WaitUntilReady();

            Setup();
            _startedOnce = true;
        }
        public void Setup()
        {
            _startScreen.Show();
            _tetrisGame.PrepareGame();
            _overdueUI.Setup();
        }

        private void Reset()
        {
            _tetrisGame.Reset();
            _overdueUI.Reset();
            _endScreen.Hide();

        }

        public void StartGame()
        {
            StartCoroutine(StartGameRoutine());

        }

        private IEnumerator StartGameRoutine()
        {
            int numOverdueBill = BillServices.GetNumOverdueBills();
            for (int i = 0; i < numOverdueBill; i++)
            {
                _overdueUI.TakeOneCross();
                yield return new WaitForSeconds(0.3f);
                _tetrisGame.AddOneStartingBlock();
                yield return new WaitForSeconds(0.3f);
            }
            yield return Tempo.WaitForABeat;
            _tetrisGame.StartGame();
        }

        private void GameOver(List<TetrisCube> cubesOverTheLine)
        {
            Debug.Log("Game Over! Cubes over the line: " + cubesOverTheLine.Count);
            StartCoroutine(GameOverRoutine(cubesOverTheLine));
        }

        private IEnumerator GameOverRoutine(List<TetrisCube> cubesOverTheLine)
        {

            yield return Tempo.WaitForABeat;

            int numCrossAdded = 0;
            foreach (var cube in cubesOverTheLine)
            {
                numCrossAdded++;
                cube.FlashColor();
                yield return new WaitForSeconds(0.6f);
                cube.Destroy();
                if (numCrossAdded > 3)
                {
                    Debug.LogWarning("Too many overdue bills, stopping the game.");
                    BillApp.GameOverAction();
                    yield break;
                }
                _overdueUI.AddOneCross();
            }
            yield return Tempo.WaitForABeat;
            _endScreen.Show(numCrossAdded, MoneyScale.GetMoneyAmount());


        }





    }
}

