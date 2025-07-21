using UnityEngine;
using Quackery.Inventories;

using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Quackery.Effects;

using DG.Tweening;
using UnityEngine.Assertions;
using Holypastry.Bakery;


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
                _cardName.text = Sprites.Replace(_item.ShortDescription); //Data.GetDescription();

                InitEffects();
                UpdatePrice();

                if (IsSkill) return;
                SetCategoryIcon();
                _cardBackground.color = Colors.Get(_item.Category.ToString());
            }
        }

        private void InitEffects()
        {
            foreach (var effect in _item.Effects)
            {
                effect.LinkedCard = this;
                if (Item.Category != EnumItemCategory.Skills)
                    effect.Tags.Add(EnumEffectTag.ItemCard);
                if (effect.Trigger == EnumEffectTrigger.Passive)
                    effect.Tags.AddUnique(EnumEffectTag.Status);

            }
        }

        private void UpdatePrice()
        {
            if (_toBeDestroyed) return;
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
                _item.Category == EnumItemCategory.Any ||
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

        public List<Effect> Effects => _item.Effects;

        private Item _item;

        public int Price => EffectServices.GetCardPrice(this) + InHandPriceBonus;

        public int InHandPriceBonus = 0;

        public bool CannotBeCovered => Effects.Exists(e => e.Data is CoverProtection);

        public bool HasCartTarget => Category != EnumItemCategory.Skills;

        private List<EffectIcon> _effectIconPool = new();
        private bool _toBeDestroyed;
        private readonly List<Effect> _activatedEffects = new();

        void Awake()
        {
            GetComponentsInChildren(true, _effectIconPool);

            SetOutline(false);
        }

        public void SetOutline(bool isOn)
        {
            if (_toBeDestroyed) return;
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
            _item.OverrideCategory = EnumItemCategory.Any;
            SetCategoryIcon();
            // Rating = _item.Rating;
            UpdatePrice();

            // _cardRating.text = Rating.ToString();
            _cardBackground.color = Colors.Get(_item.Category.ToString());

            _activatedEffects.Clear();
            _effectIconPool.ForEach(icon => icon.Activated = false);
        }

        public void UpdateUI()
        {
            if (_toBeDestroyed) return;
            UpdatePrice();
            _cardName.text = Sprites.Replace(_item.ShortDescription);
        }

        internal void Destroy()
        {
            _toBeDestroyed = true;
            transform.SetParent(null);
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject, 1f);
            });
        }



        internal void OverrideCategory(EnumItemCategory category)
        {
            if (_toBeDestroyed) return;
            _item.OverrideCategory = category;
            _cardBackground.color = Colors.Get(category.ToString());
            SetCategoryIcon();
        }


        internal void RemoveCategoryOverride()
        {
            if (_toBeDestroyed) return;
            _item.OverrideCategory = EnumItemCategory.Any;
        }

        public override string ToString()
        {
            return $"{_item.Name} - {_item.BasePrice}  - {Effects.ToString()} ";
        }
    }
}
