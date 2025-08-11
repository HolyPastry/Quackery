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
using System.Collections;


namespace Quackery.Decks
{
    public class Card : MonoBehaviour, ITooltipTarget
    {
        [Serializable]
        public struct CategoryIcons
        {
            public Sprite Icon;
            public EnumItemCategory Category;
        }
        // [Header("Catergory Icons")]
        // [SerializeField] private List<CategoryIcons> _categoryIcons = new();
        [Header("Card Components")]
        [SerializeField] private Image _base;
        [SerializeField] private Image _cardBackground;
        [SerializeField] private Image _cardForeground;

        [SerializeField] private List<Image> _categoryIcons = new();
        [SerializeField] private Image _cutoutBackground;
        // [SerializeField] private Image _PriceBackground;
        // [SerializeField] private Image _categoryIcon;
        //[SerializeField] private Image _RatingBackground;
        [SerializeField] private TextMeshProUGUI _cardTitle;

        [SerializeField] private TextMeshProUGUI _cardDescription;
        [SerializeField] private TextMeshProUGUI _cardPrice;
        //[SerializeField] private TextMeshProUGUI _cardRating;
        [SerializeField] private Image _outline;

        public List<Explanation> Explanations => Item.Data.Explanations;
        public RectTransform RectTransform => transform as RectTransform;

        public event Action<EnumItemCategory> OnCategoryChanged = delegate { };

        public Item Item
        {
            get => _item;
            set
            {
                Assert.IsNotNull(value, "Item cannot be null");
                _item = value;


                //  UpdateEffects();
                _cardForeground.sprite = _item.Data.Icon;
                _cardTitle.text = Sprites.Replace(_item.Name);
                _cardDescription.text = Sprites.Replace(_item.ShortDescription);

                InitEffects();
                UpdatePrice();
                if (IsItemCard(_item))
                    _cardBackground.color = Colors.Get(_item.Category.ToString());
                OnCategoryChanged.Invoke(_item.Category);

            }
        }

        private void InitEffects()
        {
            foreach (var effect in _item.Effects)
            {
                effect.Initialize();
                effect.LinkedCard = this;
                if (Item.Category != EnumItemCategory.Skill)
                    effect.Tags.Add(EnumEffectTag.ItemCard);
            }
        }

        public static bool IsItemCard(Item item)
        {
            return item.Category == EnumItemCategory.Ayurveda ||
                     item.Category == EnumItemCategory.Chinese ||
                     item.Category == EnumItemCategory.Crystals ||
                     item.Category == EnumItemCategory.Energy ||
                     item.Category == EnumItemCategory.Herbs ||
                        item.Category == EnumItemCategory.Magic;
        }

        private void UpdatePrice()
        {
            if (_toBeDestroyed) return;
            // _PriceBackground.gameObject.SetActive(_item.Category != EnumItemCategory.Curse);
            _cardPrice.gameObject.SetActive(IsItemCard(_item));
            _cardPrice.text = Price.ToString();

            if (Price > _item.BasePrice)
                _cardPrice.color = Color.green;
            else if (Price == _item.BasePrice)
                _cardPrice.color = Color.black;
            else if (Price < _item.BasePrice)
                _cardPrice.color = Color.red;

        }

        // private void SetCategoryIcon()
        // {
        //     if (_categoryIcon == null) return;

        //     if (
        //         _item.Category == EnumItemCategory.Any ||
        //          _item.Category == EnumItemCategory.Skill)
        //     {
        //         _categoryIcon.gameObject.SetActive(false);
        //     }
        //     else
        //     {

        //         if (_categoryIcons.Exists(icon => icon.Category == _item.Category))
        //         {
        //             _categoryIcon.gameObject.SetActive(true);
        //             _categoryIcon.sprite = _categoryIcons.Find(icon => icon.Category == _item.Category).Icon;
        //         }
        //         else
        //         {
        //             _categoryIcon.gameObject.SetActive(false);
        //             Debug.LogWarning($"No icon found for category: {_item.Category}", this);
        //         }

        //     }
        // }

        private bool IsSkill => _item.Category == EnumItemCategory.Skill;
        public string Name => _item.Name;

        public EnumItemCategory Category => _item.Category;

        public List<Effect> Effects => _item.Effects;

        private Item _item;

        public int Price => EffectServices.GetCardPrice(this) + InHandPriceBonus;

        public int InHandPriceBonus = 0;

        public bool CannotBeCovered => Effects.Exists(e => e.Data is CoverProtection);

        public bool HasCartTarget => Category != EnumItemCategory.Skill;

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
            // SetCategoryIcon();
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
            _cardDescription.text = Sprites.Replace(_item.ShortDescription);
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

            if (IsItemCard(_item))
                _cardBackground.color = Colors.Get(category.ToString());
            OnCategoryChanged.Invoke(_item.Category);
        }


        internal void RemoveCategoryOverride()
        {
            if (_toBeDestroyed) return;
            _item.OverrideCategory = EnumItemCategory.Any;
            if (IsItemCard(_item))
                _cardBackground.color = Colors.Get(_item.Category.ToString());
            OnCategoryChanged?.Invoke(_item.Category);
        }

        public override string ToString()
        {
            return $"{_item.Name} - {_item.BasePrice}  - {Effects.ToString()} ";
        }

        internal void Strip(bool stripCard)
        {
            _base.gameObject.SetActive(!stripCard);
            _cardBackground.gameObject.SetActive(!stripCard);
            _cardDescription.gameObject.SetActive(!stripCard);
            _cardTitle.gameObject.SetActive(!stripCard);

            foreach (var categoryIcon in _categoryIcons)
            {
                categoryIcon.gameObject.SetActive(!stripCard);
            }
            if (IsItemCard(_item))
            {
                _cutoutBackground.color = stripCard ? Colors.Get(_item.Category.ToString()) : Color.white;
            }

        }

        public IEnumerator PlayDestroyEffect(float duration, Material material)
        {
            _outline.gameObject.SetActive(false);

            _cardBackground.material = material;
            _cardForeground.material = material;
            _base.material = material;
            _cutoutBackground.material = material;

            material.DOFloat(1, "_disolveAmount", duration);

            yield return new WaitForSeconds(duration);
        }


    }
}
