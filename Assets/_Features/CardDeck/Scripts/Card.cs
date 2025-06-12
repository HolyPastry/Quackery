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
                    _cardBackground.color = Colors.instance.GetCategoryColor(_item.Data.Category);
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

        public Item _item;

        internal List<CardReward> CalculateCardReward(List<Card> allCards, List<CardPile> otherPiles)
        {
            return _item.CalculateCardRewards(allCards.ConvertAll(c => c.Item), otherPiles);
        }

        internal void ExecutePower(EnumPowerTrigger onCardMoveToCart, CardPile pile)
        {
            if (_item == null || _item.Data == null || _item.Data.Powers == null)
                return;

            foreach (var power in _item.Data.Powers)
            {
                if (power.Trigger == onCardMoveToCart)
                {
                    power.Execute(pile);
                }
            }
        }
    }
}
