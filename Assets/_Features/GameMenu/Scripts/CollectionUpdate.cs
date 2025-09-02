using System;
using System.Collections;
using DG.Tweening;
using Quackery.Artifacts;
using Quackery.Bills;
using Quackery.Decks;
using Quackery.Inventories;
using Quackery.TetrisBill;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    internal class CollectionUpdate : MonoBehaviour
    {
        [SerializeField] private Ease _easeIn = Ease.InCubic;
        // [SerializeField] private Ease _easeOut = Ease.OutCubic;
        [SerializeField] private float _animationDuration = 0.5f;

        [SerializeField] private Transform _parent;
        public Transform Parent => _parent;
        public RectTransform RectTransform => transform as RectTransform;

        [SerializeField] private Image _artifactImagePrefab;

        internal void DestroyItems()
        {
            foreach (Transform child in _parent)
                Destroy(child.gameObject);
        }

        internal IEnumerator MoveIn(RectTransform rectTransform)
        {
            rectTransform.SetParent(_parent);
            rectTransform.DOAnchorPos(Vector3.zero, _animationDuration).SetEase(_easeIn);
            rectTransform.DOScale(Vector3.one, _animationDuration).SetEase(_easeIn);

            yield return new WaitForSeconds(_animationDuration);
        }

        internal Card PutOut(ItemData itemData)
        {
            var newCard = DeckServices.CreateCard(itemData);
            newCard.transform.SetParent(_parent);
            newCard.transform.localScale = Vector3.one;
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.eulerAngles =
                new Vector3(0, 0, UnityEngine.Random.Range(-10f, 10f));
            return newCard;
        }

        public TetrisBlock PutOut(BillData billData)
        {
            var block = Instantiate(billData.BlockPrefab, _parent);
            block.transform.localScale = Vector3.one;
            block.transform.localPosition = Vector3.zero;
            block.SetLogo(billData.Icon);
            return block;
        }

        public Image PutOut(ArtifactData artifactData)
        {
            var image = Instantiate(_artifactImagePrefab, _parent);
            image.sprite = artifactData.Icon;
            image.transform.localScale = Vector3.one;
            image.transform.localPosition = Vector3.zero;

            return image;
        }
    }
}
