using Quackery.Decks;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class SelectionCardPool : CardPool
    {
        [SerializeField] private Button _confirmButton;

        CardPileUI _selectedCardPileUI = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            _confirmButton.onClick.AddListener(ConfirmSelection);
            _confirmButton.interactable = false;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _confirmButton.onClick.RemoveListener(ConfirmSelection);
        }

        private void ConfirmSelection()
        {
            DeckServices.SelectCard(_selectedCardPileUI.Type, _selectedCardPileUI.PileIndex);
        }

        protected override void OnCardPileTouchRelease(CardPileUI ui)
        {
            if (ui.IsEmpty) return;

            if (_selectedCardPileUI == ui)
            {
                _selectedCardPileUI = null;
                _confirmButton.interactable = false;
                return;
            }
            _confirmButton.interactable = true;

            if (_selectedCardPileUI != null)
            {
                _selectedCardPileUI.TopCard.SetOutline(false);
            }
            _selectedCardPileUI = ui;
            _selectedCardPileUI.TopCard.SetOutline(true);
        }
    }
}
