using System;
using System.Collections.Generic;
using Quackery.Inventories;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Quackery.Shops
{
    [Serializable]
    public struct ShopCardUIData
    {
        public EnumItemCategory ItemCategory;
        public Sprite BrandLogo;
        public Color BackgroundColor;
        public Sprite HighlightImage;
    }
    [CreateAssetMenu(fileName = "ShopUIData", menuName = "Quackery/Shop/ShopUIData")]
    public class ShopUIData : SerializedScriptableObject
    {
        public List<ShopCardUIData> CardUIData = new();

        public bool TryGetCardUIData(EnumItemCategory itemCategory, out (Sprite logo, Color backgroundColor, Sprite highlightImage) cardData)
        {
            cardData = default;
            if (CardUIData.Exists(data => data.ItemCategory == itemCategory) == false)
                return false;


            var foundData = CardUIData.Find(data => data.ItemCategory == itemCategory);
            cardData = (foundData.BrandLogo, foundData.BackgroundColor, foundData.HighlightImage);
            return true;
        }
    }
}
