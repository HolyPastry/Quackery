using DG.Tweening;
using UnityEngine;


namespace Quackery.TetrisBill
{
    public class BlockPool : MonoBehaviour
    {
        private const int Spacing = 100;
        public TetrisBlock AddBlock(TetrisBlock prefab)
        {
            var block = Instantiate(prefab, transform);
            block.transform.SetParent(transform);
            block.transform.localScale = Vector3.one;
            UpdateBlocksPosition();
            return block;
        }

        public bool FetchBlock(Transform newParent, out TetrisBlock block)
        {

            block = null;
            if (transform.childCount == 0) return false;

            int randomIndex = UnityEngine.Random.Range(0, transform.childCount);
            block = transform.GetChild(randomIndex).GetComponent<TetrisBlock>();
            block.transform.SetParent(newParent);
            block.transform.localScale = Vector3.one;
            UpdateBlocksPosition(animate: true);
            return true;
        }

        private void UpdateBlocksPosition(bool animate = false)
        {
            int totalWidth = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                totalWidth += (int)child.GetComponent<RectTransform>().rect.width;

            }

            totalWidth += (transform.childCount - 1) * Spacing;
            int offset = -totalWidth / 2;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                RectTransform rectTransform = child.GetComponent<RectTransform>();
                if (animate)
                {
                    rectTransform.DOAnchorPosX(offset + (rectTransform.rect.width / 2), 0.5f);
                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(offset + (rectTransform.rect.width / 2), 0);
                }

                offset += (int)rectTransform.rect.width + Spacing;
            }
        }
    }
}
