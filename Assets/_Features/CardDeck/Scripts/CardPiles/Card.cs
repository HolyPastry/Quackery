using UnityEngine;
using Quackery.Inventories;

using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Quackery.Effects;

using DG.Tweening;
using UnityEngine.Assertions;


namespace Quackery.Decks
{
    public class Card : MonoBehaviour
    {
        [Serializable]
        public struct CategoryIcons
        {
            public Sprite Icon;
            public EnumItemCategory Category;
        }
        [Header("Catergory Icons")]
        [SerializeField] private List<CategoryIcons> _categoryIcons = new();
        [Header("Card Components")]
        [SerializeField] private Image _cardBackground;
        [SerializeField] private Image _cardForeground;
        [SerializeField] private Image _PriceBackground;
        [SerializeField] private Image _categoryIcon;
        //[SerializeField] private Image _RatingBackground;
        [SerializeField] private TextMeshProUGUI _cardName;
        [SerializeField] private TextMeshProUGUI _cardPrice;
        //[SerializeField] private TextMeshProUGUI _cardRating;
        [SerializeField] private Image _outline;

        public Item Item
        {
            get => _item;
            set
            {
                Assert.IsNotNull(value, "Item cannot be null");
                _item = value;


                //  UpdateEffects();
                _cardForeground.sprite = _item.Data.Icon;
                _cardName.text = Sprites.Replace(_item.Data.ShortDescription); //Data.GetDescription();

                if (IsSkill) return;
                _cardBackground.color = Colors.Get(_item.Category.ToString());

                UpdatePrice();
                SetCategoryIcon();
            }
        }

        private void UpdatePrice()
        {

            _PriceBackground.gameObject.SetActive(_item.Category != EnumItemCategory.Curses);
            _cardPrice.gameObject.SetActive(_item.Category != EnumItemCategory.Curses);
            _cardPrice.text = Price.ToString();

            if (Price > _item.BasePrice)
                _cardPrice.color = Color.green;
            else if (Price == _item.BasePrice)
                _cardPrice.color = Color.white;
            else if (Price < _item.BasePrice)
                _cardPrice.color = Color.red;

        }

        private void SetCategoryIcon()
        {
            if (_categoryIcon == null) return;

            if (
                _item.Category == EnumItemCategory.Unset ||
                 _item.Category == EnumItemCategory.Skills)
            {
                _categoryIcon.gameObject.SetActive(false);
            }
            else
            {

                if (_categoryIcons.Exists(icon => icon.Category == _item.Category))
                {
                    _categoryIcon.gameObject.SetActive(true);
                    _categoryIcon.sprite = _categoryIcons.Find(icon => icon.Category == _item.Category).Icon;
                }
                else
                {
                    _categoryIcon.gameObject.SetActive(false);
                    Debug.LogWarning($"No icon found for category: {_item.Category}", this);
                }

            }
        }

        private bool IsSkill => _item.Category == EnumItemCategory.Skills;
        public string Name => _item.Name;

        public EnumItemCategory Category => _item.Category;

        public List<Effect> Effects => _item.Data.Effects;

        // public bool HasActivatableEffects
        // {
        //     get
        //     {
        //         foreach (var effect in Effects)
        //         {
        //             if (effect.Tags.Contains(EnumEffectTag.Activated)
        //             && !_activatedEffects.Contains(effect))
        //                 return true;

        //         }
        //         return false;
        //     }
        // }

        private Item _item;

        public int Price => EffectServices.GetCardPrice(this) + InHandPriceBonus;

        public int InHandPriceBonus = 0;

        public bool CannotBeCovered
        {
            get
            {
                return Effects.Exists(e => e.Data is CoverProtection);
            }
        }

        // public bool FullfillRequirements { get; internal set; }

        private List<EffectIcon> _effectIconPool = new();
        private readonly List<Effect> _activatedEffects = new();

        void Awake()
        {
            GetComponentsInChildren(true, _effectIconPool);

            SetOutline(false);
        }

        public void SetOutline(bool isOn)
        {
            _outline.gameObject.SetActive(isOn);
        }

        // private void UpdateEffects()
        // {
        //     Effects = new();
        //     foreach (var data in _item.Data.Effects)
        //     {
        //         Effects.Add(new Effect(data, initValue: true));
        //     }
        //     for (int i = 0; i < _effectIconPool.Count; i++)
        //     {
        //         var icon = _effectIconPool[i];
        //         if (i < Effects.Count)
        //         {
        //             icon.gameObject.SetActive(true);
        //             icon.Effect = Effects[i];
        //         }
        //         else
        //         {
        //             icon.gameObject.SetActive(false);
        //         }

        //     }
        // }

        internal List<CardReward> CalculateCardReward(List<Card> allCards, List<CardPile> otherPiles)
        {
            return _item.CalculateCardRewards(this, allCards.ConvertAll(c => c.Item), otherPiles);
        }

        public void Discard()
        {
            if (IsSkill) return;
            // Rating = _item.Rating;
            UpdatePrice();
            // _cardRating.text = Rating.ToString();
            _cardBackground.color = Colors.Get(_item.Category.ToString());

            _activatedEffects.Clear();
            _effectIconPool.ForEach(icon => icon.Activated = false);
        }

        public void UpdateUI()
        {
            UpdatePrice();
        }

        // internal void ExecutePowerInCart(CardPile pile)
        // {
        //     foreach (var effect in Effects)
        //     {
        //         if (effect.Tags.Contains(EnumEffectTag.Activated)) continue;
        //         if (effect.Trigger == EnumEffectTrigger.OnCardPlayed)
        //             effect.Execute(pile);
        //         if (effect.Trigger == EnumEffectTrigger.OnDraw ||
        //              effect.Trigger == EnumEffectTrigger.Continous)
        //             EffectServices.Add(effect);
        //     }
        // }

        internal void Destroy()
        {
            transform.SetParent(null);
            transform.DOMoveX(Screen.width * 2, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject, 1f);
            });
        }

        // internal void ActivatePower(CardPile lastCartPile)
        // {
        //     foreach (var effect in Effects)
        //     {
        //         if (!effect.Tags.Contains(EnumEffectTag.Activated)) continue;
        //         if (_activatedEffects.Contains(effect)) continue;

        //         effect.LinkedCard = lastCartPile.TopCard;
        //         EffectServices.Add(effect);
        //         _activatedEffects.Add(effect);
        //         var effectIcon = _effectIconPool.Find(icon => icon.Effect == effect);
        //         effectIcon.Activated = true;
        //         if (effect.Trigger == EnumEffectTrigger.OnActivated)
        //             effect.Execute(lastCartPile);
        //     }
        // }

        internal void OverrideCategory(EnumItemCategory category)
        {
            _item.OverrideCategory = category;
            _cardBackground.color = Colors.Get(category.ToString());
            SetCategoryIcon();
        }


        internal void RemoveCategoryOverride()
        {
            _item.OverrideCategory = EnumItemCategory.Unset;
        }

        public override string ToString()
        {
            return $"{_item.Name} - {_item.BasePrice}  - {Effects.ToString()} ";
        }
    }
}
