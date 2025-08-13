
#if UNITY_EDITOR
using System;
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
        [Header("Paths")]
        [SerializeField] private string _itemPath = "Assets/_Content/2DArt/Sprites/CardItems";
        [SerializeField] private string _skillPath = "Assets/_Content/2DArt/Sprites/Skills";
        [SerializeField] private string _cursePath = "Assets/_Content/2DArt/Sprites/Curses";
        [SerializeField] private string _tempCursePath = "Assets/_Content/2DArt/Sprites/TempCurses";

        [Header("Collections Keys")]
        [SerializeField] private string EffectCollectionKey = "Effects";
        [SerializeField] private string ExplanationCollectionKey = "Explanations";

        [Header("Card Prefixes")]
        [SerializeField] private string _cardPrefix = "CardType=";

        private DataCollection<EffectData> _effectCollection;
        private DataCollection<Explanation> _explanationCollection;

        public event Action<string> OnPopulated = delegate { };

        protected override void Populate(List<string> fields, ScriptableObject @object, int IndexOf)
        {
            string log = string.Empty;
            ItemData itemData = @object as ItemData;
            itemData.MasterText = fields[3];
            itemData.Category = (EnumItemCategory)Enum.Parse(typeof(EnumItemCategory), fields[6]);
            itemData.BasePrice = IntParse(fields[7]);
            itemData.Rarity = (EnumRarity)Enum.Parse(typeof(EnumRarity), fields[4]);
            itemData.Icon = GetIcon(fields[2], itemData.Category, out string iconLog);
            log += iconLog;


            itemData.SubscriptionCost = IntParse(fields[8]);

            int index = 9;
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
                    log += $" - Effect: '{effectName}'\n";
                    continue;
                }
                var newEffect = new Effect(effectData, effectValue);

                //  itemData.Effects.Add(newEffect);
            }
            var explanations = fields[index++];
            ParseExplanations(itemData, explanations);
            itemData.ShortDescription = fields[index++];
            itemData.LongDescription = fields[index++];
            itemData.ShopDescription = fields[index++];

            OnPopulated?.Invoke(log);

        }

        private Sprite GetIcon(string name, EnumItemCategory category, out string log)
        {
            log = string.Empty;
            string path = category switch
            {
                EnumItemCategory.Skill => _skillPath,
                EnumItemCategory.Curse => _cursePath,
                EnumItemCategory.TempCurse => _tempCursePath,
                _ => _itemPath
            };
            string prefix = _cardPrefix;
            if (category == EnumItemCategory.Curse ||
                 category == EnumItemCategory.TempCurse ||
                 category == EnumItemCategory.Skill)
            {
                prefix = category.ToString() + '=';
            }


            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Join(path, prefix + name + ".png"));
            if (sprite == null)
                log += $" - Asset: {path}/{prefix}{name}.png\n";

            return sprite;

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