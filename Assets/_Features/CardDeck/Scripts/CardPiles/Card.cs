using UnityEngine;
using Quackery.Inventories;

using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Quackery.Effects;
using System.Collections;
using DG.Tweening;
using UnityEngine.Assertions;

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
        [SerializeField] private Image _outline;

        public Item Item

        {
            get => _item;
            set
            {
                Assert.IsNotNull(value, "Item cannot be null");
                _item = value;

                UpdateEffects();
                _cardForeground.sprite = _item.Data.Icon;
                _cardName.text = _item.Data.name;

                if (IsSkill) return;

                Price = _item.Price;
                Rating = _item.Rating;
                _cardBackground.color = ColorLibrary.instance.Get(_item.Data.Category.ToString());

                _cardPrice.text = Price.ToString();
                _cardRating.text = Rating.ToString();
            }
        }

        private bool IsSkill => _item.Data.Category == EnumItemCategory.Skills;
        public string Name => _item.Name;

        public EnumItemCategory Category => _item.Data.Category;

        public List<Effect> Effects { get; private set; }

        public bool HasActivatableEffects
        {
            get
            {
                foreach (var effect in Effects)
                {
                    if (effect.Data.Tags.Contains(EnumEffectTag.Activated)
                    && !_activatedEffects.Contains(effect))
                        return true;
                }
                return false;
            }
        }

        private Item _item;

        public int Price { get; private set; }
        public int Rating { get; private set; }


        private List<EffectIcon> _effectIconPool = new();
        private Material _material;
        private readonly List<Effect> _activatedEffects = new();

        void Awake()
        {
            GetComponentsInChildren(true, _effectIconPool);

            SetInteractive(false);
        }

        public void SetInteractive(bool isOn)
        {
            _outline.gameObject.SetActive(isOn);
        }

        private void UpdateEffects()
        {
            Effects = new();
            foreach (var data in _item.Data.Effects)
            {
                Effects.Add(new Effect(data, initValue: true));
            }
            for (int i = 0; i < _effectIconPool.Count; i++)
            {
                var icon = _effectIconPool[i];
                if (i < Effects.Count)
                {
                    icon.gameObject.SetActive(true);
                    icon.Effect = Effects[i];

                }
                else
                {
                    icon.gameObject.SetActive(false);
                }

            }
        }

        internal List<CardReward> CalculateCardReward(List<Card> allCards, List<CardPile> otherPiles)
        {
            return _item.CalculateCardRewards(this, allCards.ConvertAll(c => c.Item), otherPiles);
        }

        public void Discard()
        {
            if (IsSkill) return;
            Price = _item.Price;
            Rating = _item.Rating;
            _cardPrice.text = Price.ToString();
            _cardRating.text = Rating.ToString();
            _activatedEffects.Clear();
            _effectIconPool.ForEach(icon => icon.Activated = false);
        }

        public void CheckEffects()
        {
            int priceModifier = EffectServices.GetPriceModifier(this);
            int ratingModifier = EffectServices.GetRatingModifier(this);
            Price = _item.Price + priceModifier;
            Rating = _item.Rating + ratingModifier;
            _cardPrice.text = Price.ToString();
            _cardRating.text = Rating.ToString();
        }

        internal void ExecutePowerInCart(CardPile pile)
        {
            foreach (var effect in Effects)
            {
                if (effect.Data.Tags.Contains(EnumEffectTag.Activated)) continue;
                if (effect.Trigger == EnumEffectTrigger.OnCardMoveToCart)
                    effect.Execute(pile);
                if (effect.Trigger == EnumEffectTrigger.OnDraw ||
                     effect.Trigger == EnumEffectTrigger.Continous)
                    EffectServices.Add(effect);
            }
        }

        internal void Destroy()
        {
            transform.SetParent(null);
            transform.DOMoveX(Screen.width * 2, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject, 1f);
            });
        }

        internal void ActivatePower(CardPile lastCartPile)
        {
            foreach (var effect in Effects)
            {
                if (!effect.Data.Tags.Contains(EnumEffectTag.Activated)) continue;
                if (_activatedEffects.Contains(effect)) continue;

                effect.LinkedCard = lastCartPile.TopCard;
                EffectServices.Add(effect);
                _activatedEffects.Add(effect);
                var effectIcon = _effectIconPool.Find(icon => icon.Effect == effect);
                effectIcon.Activated = true;
                if (effect.Trigger == EnumEffectTrigger.OnActivated)
                    effect.Execute(lastCartPile);
            }
        }
    }
}
