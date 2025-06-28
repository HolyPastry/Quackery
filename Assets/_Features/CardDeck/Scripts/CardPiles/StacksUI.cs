

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
        }
        void OnDisable()
        {
            _cartPileUI.OnCardMovedIn -= UpdateUI;
            _cartPileUI.OnCardMovedOut -= UpdateUI;
            _cartPileUI.OnPileUpdated -= UpdateUI;
        }

        private void UpdateUI()
        {
            if (_cartPileUI.IsEmpty)
            {
                ClearCounts();
                return;
            }

            var cards = _cartPileUI.GetComponentsInChildren<Card>();
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
