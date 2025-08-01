using DG.Tweening;
using UnityEngine;


namespace Quackery.TetrisBill
{
    public class BlockPool : MonoBehaviour
    {
        [SerializeField] private RectTransform _blockParent;
        [SerializeField] private int _spacing = 100;
        public TetrisBlock SpawnBlock(TetrisBlock prefab)
        {
            var block = Instantiate(prefab, _blockParent);
            block.transform.SetParent(_blockParent);
            block.transform.localScale = Vector3.one;
            UpdateBlocksPosition();
            return block;
        }

        public bool FetchBlock(Transform newParent, out TetrisBlock block)
        {

            block = null;
            if (_blockParent.childCount == 0) return false;

            int randomIndex = UnityEngine.Random.Range(0, _blockParent.childCount);
            block = _blockParent.GetChild(randomIndex).GetComponent<TetrisBlock>();
            block.transform.SetParent(newParent);
            block.transform.localScale = Vector3.one;
            UpdateBlocksPosition(animate: true);
            return true;
        }

        private void UpdateBlocksPosition(bool animate = false)
        {
            int totalWidth = 0;
            for (int i = 0; i < _blockParent.childCount; i++)
            {
                Transform child = _blockParent.GetChild(i);
                totalWidth += (int)child.GetComponent<RectTransform>().rect.width;

            }

            totalWidth += (_blockParent.childCount - 1) * _spacing;
            int offset = -totalWidth / 2;
            for (int i = 0; i < _blockParent.childCount; i++)
            {
                Transform child = _blockParent.GetChild(i);
                RectTransform rectTransform = child.GetComponent<RectTransform>();
                if (animate)
                {
                    rectTransform.DOAnchorPosX(offset + (rectTransform.rect.width / 2), 0.5f);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(offset + (rectTransform.rect.width / 2), 0);
                }

                offset += (int)rectTransform.rect.width + _spacing;
            }
            _blockParent.sizeDelta = new Vector2(totalWidth, _blockParent.sizeDelta.y);
        }
    }
}
