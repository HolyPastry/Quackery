using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Quackery.TetrisBill
{
    public class TetrisGame : MonoBehaviour
    {
        [SerializeField] private int _gridWidth = 10;
        [SerializeField] private int _gridHeight = 16;
        [SerializeField] private int _cellSize = 100;
        [SerializeField] private float _speed = 1.0f;
        [SerializeField] private BlockPool _shapePool;

        private int _spawnOffset => _cellSize * _gridHeight;



        private List<TetrisCube> _cubes = new();

        [SerializeField] private List<TetrisBlock> _tetrisBlockPrefab;

        private TetrisBlock _currentBlock;


        internal static Action MoveRight = delegate { };
        internal static Action MoveLeft = delegate { };
        internal static Action Rotate = delegate { };
        public static Func<int> CellSize = () => 1;

        private Vector2 _moveDirection;
        private bool _needRotation;
        private bool _removingLines;

        void Awake()
        {
            GetComponentsInChildren(true, _cubes);
        }

        void OnEnable()
        {
            MoveRight = () => _moveDirection = Vector2.right;
            MoveLeft = () => _moveDirection = Vector2.left;
            Rotate = () => _needRotation = true;
            CellSize = () => _cellSize;
        }



        void OnDisable()
        {
            MoveRight = delegate { };
            MoveLeft = delegate { };
            Rotate = delegate { };
            CellSize = null;
        }



        public void StartGame()
        {



            StartCoroutine(GameLoopRoutine());
        }

        private IEnumerator GameLoopRoutine()
        {
            yield return StartCoroutine(SpawnAllBlocks());
            yield return new WaitForSeconds(0.5f);
            while (true)
            {
                yield return null;
                if (_removingLines) continue;
                if (_currentBlock == null)
                {
                    var block = FetchNewBlock();
                    if (block == null)
                        yield break; // No more blocks to spawn
                    yield return null;
                    _currentBlock = block;
                }
                else
                    MoveCurrentBlock();
            }

        }

        private IEnumerator SpawnAllBlocks()
        {
            var bills = BillServices.GetAllBills();

            foreach (var bill in bills)
            {
                var block = _shapePool.AddBlock(bill.Data.BlockPrefab);
                block.SetLogo(bill.Data.Icon);
                yield return new WaitForSeconds(0.2f); // Small delay to visualize the spawning
            }

        }

        private void RotateCurrentBlock()
        {
            _currentBlock.Rotate(90);
            if (_currentBlock.OverlapOtherCubes(_cubes))
                _currentBlock.Rotate(-90);

        }
        private void MoveCurrentBlock()
        {

            if (_needRotation)
            {
                RotateCurrentBlock();
                _needRotation = false;
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

            if (_currentBlock.Collided)
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


    }
}

