using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Bills;
using UnityEngine;

namespace Quackery.TetrisBill
{
    public class TetrisController : MonoBehaviour
    {
        [SerializeField] private TetrisGame _tetrisGame;
        [SerializeField] private TetrisStartScreen _startScreen;
        [SerializeField] private TetrisOverdueUI _overdueUI;
        private bool _startedOnce;

        void OnEnable()
        {
            _startScreen.OnStart += StartGame;
            _tetrisGame.OnGameOver += GameOver;
            if (_startedOnce) Setup();


        }

        void OnDisable()
        {
            _startScreen.OnStart -= StartGame;
            _tetrisGame.OnGameOver -= GameOver;
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
            yield return new WaitForSeconds(0.5f);
            _tetrisGame.StartGame();
        }

        private void GameOver(List<TetrisCube> cubesOverTheLine)
        {
            Debug.Log("Game Over! Cubes over the line: " + cubesOverTheLine.Count);
            StartCoroutine(GameOverRoutine(cubesOverTheLine));
        }

        private IEnumerator GameOverRoutine(List<TetrisCube> cubesOverTheLine)
        {

            yield return new WaitForSeconds(1f);
            int budget = MoneyScale.GetBudgetAmount();
            PurseServices.Modify(-budget);

            yield return new WaitForSeconds(1f);
            int numCrossAdded = 0;
            foreach (var cube in cubesOverTheLine)
            {
                numCrossAdded++;
                cube.FlashColor();
                yield return new WaitForSeconds(0.5f);
                cube.Destroy();
                if (numCrossAdded > 3)
                {
                    Debug.LogWarning("Too many overdue bills, stopping the game.");
                    BillApp.GameOverAction();
                    yield break;
                }
                _overdueUI.AddOneCross();
            }
            yield return new WaitForSeconds(0.5f);
            if (numCrossAdded > 0)
            {
                BillServices.SetNumOverdueBills(numCrossAdded);
                yield return StartCoroutine(_overdueUI.ActOverdueBillRoutine());
            }
            yield return new WaitForSeconds(1f);
            BillApp.ContinueAction();
        }
    }
}

