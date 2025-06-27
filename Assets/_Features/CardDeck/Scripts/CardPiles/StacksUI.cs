
using KBCore.Refs;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{
    public class StacksUI : ValidatedMonoBehaviour
    {
        [SerializeField, Parent] private CardPileUI _cardPileUI;
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
            _cardPileUI.OnCardMovedIn += UpdateUI;
            _cardPileUI.OnCardMovedOut += UpdateUI;
            _cardPileUI.OnPileUpdated += UpdateUI;
        }
        void OnDisable()
        {
            _cardPileUI.OnCardMovedIn -= UpdateUI;
            _cardPileUI.OnCardMovedOut -= UpdateUI;
            _cardPileUI.OnPileUpdated -= UpdateUI;
        }

        private void UpdateUI()
        {
            if (_cardPileUI.IsEmpty)
            {
                ClearCounts();
                return;
            }

            var cards = _cardPileUI.GetComponentsInChildren<Card>();
            int herbCount = 0;
            int magicCount = 0;
            int chineseCount = 0;
            int crystalCount = 0;

            for (int i = 0; i < cards.Length - 1; i++)
            {
                var card = cards[i];
                if (card == null) continue;

                switch (card.Category)
                {
                    case EnumItemCategory.Herbs:
                        herbCount++;
                        break;
                    case EnumItemCategory.Magic:
                        magicCount++;
                        break;
                    case EnumItemCategory.Chinese:
                        chineseCount++;
                        break;
                    case EnumItemCategory.Crystals:
                        crystalCount++;
                        break;
                }
            }

            SetCounts(herbCount, magicCount, chineseCount, crystalCount);

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
