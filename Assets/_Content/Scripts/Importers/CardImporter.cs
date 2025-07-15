
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Bakery.GoogleSheet;
using Holypastry.Bakery;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEditor;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "CardImporter", menuName = "Quackery/GoogleSheet/CardImporter")]
    public class CardImporter : DataImporter<ItemData>
    {
        [SerializeField] private string _itemPath = "Assets/_Content/2DArt/Sprites/CardItems";
        [SerializeField] private string _skillPath = "Assets/_Content/2DArt/Sprites/Skills";
        [SerializeField] private string EffectCollectionKey = "Effects";
        [SerializeField] private string ExplanationCollectionKey = "Explanations";
        [SerializeField] private string _cardPrefix = "CardType=";
        [SerializeField] private string _skillPrefix = "Skill=";

        private DataCollection<EffectData> _effectCollection;
        private DataCollection<Explanation> _explanationCollection;
        protected override void Populate(List<string> fields, ScriptableObject @object, int IndexOf)
        {
            ItemData itemData = @object as ItemData;
            itemData.MasterText = fields[3];
            itemData.Category = (EnumItemCategory)System.Enum.Parse(typeof(EnumItemCategory), fields[5]);
            itemData.BasePrice = IntParse(fields[6]);
            if (itemData.Category == EnumItemCategory.Skills)
            {
                itemData.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Join(_skillPath, _skillPrefix + fields[2] + ".png"));
            }
            else
            {
                itemData.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Join(_itemPath, _cardPrefix + fields[2] + ".png"));
            }

            itemData.SubscriptionCost = IntParse(fields[7]);

            int index = 8;
            itemData.Effects.Clear();
            for (int i = 0; i < 3; i++)
            {
                var effectName = fields[index++];
                int effectValue = IntParse(fields[index++]);

                if (string.IsNullOrEmpty(effectName))
                    continue;
                _effectCollection ??= new(EffectCollectionKey);
                var effectData = _effectCollection.GetFromName(effectName);
                if (effectData == null)
                {
                    Debug.LogWarning($"Effect '{effectName}' not found in collection '{EffectCollectionKey}' for item '{itemData.name}' at index {IndexOf}.");
                    //continue;
                }
                var newEffect = new Effect(effectData)
                {
                    Value = effectValue
                };
                itemData.Effects.Add(newEffect);
            }
            var explanations = fields[index++];
            ParseExplanations(itemData, explanations);
            itemData.ShortDescription = fields[index++];
            itemData.LongDescription = fields[index++];
            itemData.ShopDescription = fields[index++];

        }

        private void ParseExplanations(ItemData itemData, string explanations)
        {
            itemData.Explanations.Clear();
            foreach (var explanationName in explanations.Split(','))
            {
                if (string.IsNullOrEmpty(explanationName))
                    continue;
                _explanationCollection ??= new(ExplanationCollectionKey);
                var explanation = _explanationCollection.GetFromName(explanationName.Trim());
                if (explanation != null)
                {
                    itemData.Explanations.Add(explanation);
                }
                else
                {
                    Debug.LogWarning($"Explanation '{explanationName}' not found for item '{itemData.name}'.");
                }
            }
        }
    }


}

#endif