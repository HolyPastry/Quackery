using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.TetrisBill
{
    public class TetrisBlock : TetrisElement
    {
        [SerializeField] private Image _logo;
        [SerializeField] private bool _isOdd = false;
        public bool Collided => _cubes.Any(c => c.Collided);
        public List<TetrisCube> Cubes => _cubes;
        public bool IsOdd => _isOdd;
        private List<TetrisCube> _cubes = new();



        void Awake()
        {
            GetComponentsInChildren(true, _cubes);
        }

        internal bool OverlapOtherCubes(List<TetrisCube> allCubes)
        {
            foreach (var cube in _cubes)
                if (allCubes.Any(c => c != cube && c.Overlaps(cube)))
                    return true;

            return false;
        }

        internal void Rotate(int v) =>
            rectTransform.Rotate(0, 0, v);

        internal void SnapToGrid(UnityEngine.Transform transform)
        {
            _cubes.ForEach(c =>
            {
                c.transform.SetParent(transform);
                c.PositionY = c.LineIndex * TetrisGame.CellSize();
            });
        }

        public void SetLogo(Sprite logo)
        {
            _logo.sprite = logo;
            _logo.enabled = logo != null;
        }
    }
}