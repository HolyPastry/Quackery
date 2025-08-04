using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Quackery.TetrisBill
{
    public class TetrisGame : MonoBehaviour
    {
        [SerializeField] private int _gridWidth = 10;
        [SerializeField] private int _gridHeight = 16;
        [SerializeField] private int _cellSize = 100;
        [SerializeField] private float _normalSpeed = 1.0f;
        [SerializeField] private float _fastFallSpeed = 5.0f;
        [SerializeField] private BlockPool _shapePool;

        [SerializeField] private TetrisCube _startingCubePrefab;

        private int _spawnOffset => _cellSize * _gridHeight;

        public Action<List<TetrisCube>> OnGameOver = delegate { };

        private List<TetrisCube> _cubes = new();
        private List<int> _startingCubeIndexes = new();

        private TetrisBlock _currentBlock;
        internal static Action MoveRight = delegate { };
        internal static Action MoveLeft = delegate { };
        internal static Action<int> Rotate = delegate { };
        internal static Action<bool> SetFastFall = delegate { };
        internal static Action<int> SetBudgetIndex = delegate { };
        public static Func<int> CellSize = () => 1;

        private Vector2 _moveDirection;
        private int _needRotation = 0;
        private bool _removingLines;
        private int _budgetIndex;

        private float _speed => _fastFallEnabled ? _fastFallSpeed : _normalSpeed;
        private bool _fastFallEnabled = false;

        void OnEnable()
        {
            MoveRight = () => _moveDirection = Vector2.right;
            MoveLeft = () => _moveDirection = Vector2.left;
            Rotate = (direction) => _needRotation = direction;
            SetBudgetIndex = (index) => _budgetIndex = index;
            CellSize = () => _cellSize;
            SetFastFall = (isOn) => _fastFallEnabled = isOn;

        }



        void OnDisable()
        {
            MoveRight = delegate { };
            MoveLeft = delegate { };
            Rotate = delegate { };
            SetBudgetIndex = delegate { };
            CellSize = () => 1;
            SetFastFall = delegate { };
        }


        public void PrepareGame()
        {
            _cubes.Clear();
            GetComponentsInChildren(true, _cubes);
            _startingCubeIndexes.Clear();
            StartCoroutine(SpawnAllBlocks());
        }
        public void StartGame()
        {

            StartCoroutine(GameLoopRoutine());
        }

        private IEnumerator GameLoopRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            while (true)
            {
                yield return null;
                if (_removingLines) continue;
                if (_currentBlock == null)
                {
                    var block = FetchNewBlock();
                    if (block == null)
                        break; // No more blocks to spawn
                    yield return null;
                    _currentBlock = block;
                }
                else
                    MoveCurrentBlock();
            }
            OnGameOver(GetCubesAboveLineIndex());
        }

        private List<TetrisCube> GetCubesAboveLineIndex()
        {
            return _cubes.FindAll(c => c.LineIndex > _budgetIndex);
        }

        private IEnumerator SpawnAllBlocks()
        {
            yield return FlowServices.WaitUntilEndOfSetup();
            yield return BillServices.WaitUntilReady();
            var bills = BillServices.GetAllBills();

            foreach (var bill in bills)
            {
                var block = _shapePool.SpawnBlock(bill.Data.BlockPrefab);
                block.SetLogo(bill.Data.Icon);
                block.name = $"Block_{bill.Data.MasterText}";
                yield return new WaitForSeconds(0.2f); // Small delay to visualize the spawning
            }

        }

        private void RotateCurrentBlock()
        {
            _currentBlock.Rotate(90 * _needRotation);
            if (_currentBlock.OverlapOtherCubes(_cubes))
                _currentBlock.Rotate(-90 * _needRotation); // Revert the rotation if it overlaps with other cubes

        }
        private void MoveCurrentBlock()
        {

            if (_needRotation != 0)
            {
                RotateCurrentBlock();
                _needRotation = 0;
                return;
            }

            if (_moveDirection != Vector2.zero)
            {
                MoveSideWayHandler(_moveDirection);
                _moveDirection = Vector2.zero;
                return;
            }

            MoveCurrentBlockDown();
        }

        private void MoveSideWayHandler(Vector2 direction)
        {

            _currentBlock.PositionX += direction.x * _cellSize;

            if (_currentBlock.OverlapOtherCubes(_cubes))
                _currentBlock.PositionX -= direction.x * _cellSize;
        }


        private void MoveCurrentBlockDown()
        {

            var nextPositionDelta = _speed * Time.deltaTime * Vector2.down;
            _currentBlock.PositionY += nextPositionDelta.y;

            if (_currentBlock.OverlapOtherCubes(_cubes))
            {
                _currentBlock.PositionY -= nextPositionDelta.y; // Revert the position
                PlaceCurrentBlock();
            }
        }

        private void PlaceCurrentBlock()
        {
            _removingLines = true;

            _cubes.AddRange(_currentBlock.Cubes);

            _currentBlock.SnapToGrid(transform);
            StartCoroutine(RemoveCompletedLines());
            Destroy(_currentBlock.gameObject);

            _currentBlock = null;
        }

        private bool CheckCompletedLineRoutine(out int completedLineIndex)
        {
            completedLineIndex = -1;
            for (int lineIndex = 0; lineIndex < _gridHeight; lineIndex++)
            {
                if (_cubes.Count(c =>
                        !c.IsBorder &&
                        c.LineIndex == lineIndex) < _gridWidth) continue;

                completedLineIndex = lineIndex;
                return true;
            }
            return false;

        }

        private IEnumerator RemoveCompletedLines()
        {

            while (CheckCompletedLineRoutine(out int lineIndex))
            {

                var cubesToRemove = _cubes.FindAll(c => !c.IsBorder && c.LineIndex == lineIndex);
                foreach (var cube in cubesToRemove)
                {
                    _cubes.Remove(cube);
                    cube.FlashColor();
                }
                yield return new WaitForSeconds(0.5f);
                foreach (var cube in cubesToRemove)
                {
                    cube.Destroy();
                }

                // Move all cubes above down
                foreach (var cube in _cubes.Where(c => !c.IsBorder && c.LineIndex > lineIndex))
                {
                    cube.MoveDownOne(0.2f);
                }
                yield return new WaitForSeconds(0.2f);
            }
            int highestLineIndex = _cubes.Max(c => c.LineIndex);
            MoneyScale.SetMoneyAmount(highestLineIndex);
            _removingLines = false;

        }

        private TetrisBlock FetchNewBlock()
        {
            if (!_shapePool.FetchBlock(transform, out TetrisBlock block))
                return null;

            block.Position = new Vector2(
               GetSpawnXPosition() + ((block.IsOdd) ? _cellSize / 2 : 0),
                _spawnOffset + _cellSize
            );

            return block;
        }

        private float GetSpawnXPosition()
        {
            return 0;
            //return UnityEngine.Random.Range(-_gridWidth / 2, _gridWidth / 2);
        }

        internal void AddOneStartingBlock()
        {
            int randomIndex = UnityEngine.Random.Range(0, _gridWidth);
            while (_startingCubeIndexes.Contains(randomIndex))
            {
                randomIndex = UnityEngine.Random.Range(0, _gridWidth);
            }
            _startingCubeIndexes.Add(randomIndex);
            var cube = Instantiate(_startingCubePrefab, transform);
            cube.PositionX = (randomIndex - _gridWidth / 2 + 0.5f) * _cellSize;
            cube.PositionY = 0;

            _cubes.Add(cube);
        }

        internal void Reset()
        {
            _cubes.Where(c => c != null && !c.IsBorder)
                 .ToList()
                 .ForEach(c =>
                    {
                        _cubes.Remove(c);
                        Destroy(c.gameObject);
                    });
        }
    }
}

