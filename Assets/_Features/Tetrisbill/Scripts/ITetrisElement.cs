using UnityEngine;


namespace Quackery.TetrisBill
{
    public abstract class TetrisElement : MonoBehaviour
    {
        public RectTransform rectTransform => transform as RectTransform;
        public Vector2 Position
        {
            get => rectTransform.anchoredPosition;
            set => rectTransform.anchoredPosition = value;
        }
        public float PositionY
        {
            get => rectTransform.anchoredPosition.y;
            set => rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value);
        }
        public float PositionX
        {
            get => rectTransform.anchoredPosition.x;
            set => rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y);
        }

        public int LineIndex => Mathf.RoundToInt(PositionY / TetrisGame.CellSize());
    }
}