using UnityEngine;
using Quackery.Inventories;

using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Quackery.Decks
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image _cardBackground;
        [SerializeField] private Image _cardForeground;
        [SerializeField] private Image _PriceBackground;
        [SerializeField] private Image _RatingBackground;
        [SerializeField] private TextMeshProUGUI _cardName;
        [SerializeField] private TextMeshProUGUI _cardPrice;
        [SerializeField] private TextMeshProUGUI _cardRating;

        public Item Item

        {
            get => _item;
            set
            {
                _item = value;
                if (_item != null)
                {
                    _cardBackground.color = ColorManager.Category(_item.Data.Category);
                    _cardForeground.sprite = _item.Data.Icon;
                    _cardName.text = _item.Data.name;
                    _cardPrice.text = _item.Price.ToString();
                    _cardRating.text = _item.Rating.ToString();
                }
                else
                {
                    _cardBackground.color = Color.clear;
                    _cardForeground.sprite = null;
                    _cardName.text = string.Empty;
                    _cardPrice.text = string.Empty;
                    _cardRating.text = string.Empty;

                }
            }
        }
        public string Name => _item.Name;

        public EnumItemCategory Category => _item.Data.Category;

        public List<Power> Powers => _item.Data.Powers;

        public bool HasActivatablePowers
        {
            get
            {
                if (_item == null || _item.Data == null || _item.Data.Powers == null)
                    return false;

                foreach (var power in _item.Data.Powers)
                {
                    if (power.Trigger == EnumPowerTrigger.Activated
                    && !_activatedPowers.Contains(power))
                        return true;
                }
                return false;
            }
        }

        public Item _item;

        private List<PowerIcon> _powerIcons = new();
        private readonly List<Power> _activatedPowers = new();

        void Awake()
        {
            GetComponentsInChildren(true, _powerIcons);
        }

        void Start()
        {
            _powerIcons.ForEach(icon => icon.Show(_item.Data.Powers));
        }

        internal List<CardReward> CalculateCardReward(List<Card> allCards, List<CardPile> otherPiles)
        {
            return _item.CalculateCardRewards(allCards.ConvertAll(c => c.Item), otherPiles);
        }

        public void Discard()
        {

            _activatedPowers.Clear();
            _powerIcons.ForEach(icon => icon.Reset());
        }

        internal void ExecutePower(EnumPowerTrigger trigger, CardPile pile)
        {
            if (_item == null || _item.Data == null || _item.Data.Powers == null)
                return;

            foreach (var power in _item.Data.Powers)
            {
                if (power.Trigger == trigger)
                {
                    power.Execute(pile);
                    if (power.Trigger == EnumPowerTrigger.Activated)
                    {
                        _activatedPowers.Add(power);
                        _powerIcons.ForEach(icon => icon.SetActive(power, true));
                    }
                }
            }
        }
    }
}
