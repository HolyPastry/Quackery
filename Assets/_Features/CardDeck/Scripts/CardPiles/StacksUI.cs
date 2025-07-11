

using System.Collections.Generic;
using System.Linq;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public class StacksUI : MonoBehaviour
    {
        [SerializeField] private CartPileUI _cartPileUI;
        [SerializeField] private StackInfoUI _herbStackInfo;
        [SerializeField] private StackInfoUI _magicStackInfo;
        [SerializeField] private StackInfoUI _chineseStackInfo;
        [SerializeField] private StackInfoUI _crystalStackInfo;

        void Awake()
        {
            ClearCounts();
        }

        void OnEnable()
        {
            _cartPileUI.OnCardMovedIn += UpdateUI;
            _cartPileUI.OnCardMovedOut += UpdateUI;
            _cartPileUI.OnPileUpdated += UpdateUI;
            CartEvents.OnCartCleared += ClearCounts;
        }
        void OnDisable()
        {
            _cartPileUI.OnCardMovedIn -= UpdateUI;
            _cartPileUI.OnCardMovedOut -= UpdateUI;
            _cartPileUI.OnPileUpdated -= UpdateUI;
            CartEvents.OnCartCleared -= ClearCounts;
        }

        private void UpdateUI()
        {
            if (_cartPileUI.IsEmpty)
            {
                ClearCounts();
                return;
            }
            List<Card> cards = new();
            _cartPileUI.GetComponentsInChildren(true, cards);

            if (cards.Count == 0)
            {
                ClearCounts();
                return;
            }

            int herbCount = cards.Sum(card => card.Category == EnumItemCategory.Herbs ? 1 : 0);
            int magicCount = cards.Sum(card => card.Category == EnumItemCategory.Magic ? 1 : 0);
            int chineseCount = cards.Sum(card => card.Category == EnumItemCategory.Chinese ? 1 : 0);
            int crystalCount = cards.Sum(card => card.Category == EnumItemCategory.Crystals ? 1 : 0);

            SetCounts(herbCount, magicCount, chineseCount, crystalCount);

            if (cards.TrueForAll(card => cards[0].Category == card.Category))
            {
                _herbStackInfo.SetSynergy(cards[0].Category == EnumItemCategory.Herbs);
                _magicStackInfo.SetSynergy(cards[0].Category == EnumItemCategory.Magic);
                _chineseStackInfo.SetSynergy(cards[0].Category == EnumItemCategory.Chinese);
                _crystalStackInfo.SetSynergy(cards[0].Category == EnumItemCategory.Crystals);
            }

        }

        public void SetCounts(int herbCount, int magicCount, int chineseCount, int crystalCount)
        {
            _herbStackInfo.SetCount(herbCount);
            _magicStackInfo.SetCount(magicCount);
            _chineseStackInfo.SetCount(chineseCount);
            _crystalStackInfo.SetCount(crystalCount);
        }
        public void ClearCounts()
        {
            _herbStackInfo.ClearCount();
            _magicStackInfo.ClearCount();
            _chineseStackInfo.ClearCount();
            _crystalStackInfo.ClearCount();
        }
    }
}
