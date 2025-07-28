
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Bakery.GoogleSheet;
using Holypastry.Bakery;
using Quackery.Artifacts;
using Quackery.Bills;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEditor;
using UnityEngine;

namespace Quackery
{


    [CreateAssetMenu(fileName = "ArtifactImporter", menuName = "Quackery/GoogleSheet/ArtifactImporter")]
    public class ArtifactImporter : DataImporter<ArtifactData>
    {
        [SerializeField] private string _iconPath = "Assets/_Content/2DArt/Sprites/Artifacts";

        [SerializeField] private string EffectCollectionKey = "Effects";
        [SerializeField] private string ExplanationCollectionKey = "Explanations";
        [SerializeField] private string BillCollectionKey = "Bills";
        [SerializeField] private string CardCollectionKey = "Cards";


        private DataCollection<EffectData> _effectCollection;
        private DataCollection<Explanation> _explanationCollection;
        private DataCollection<BillData> _billCollection;
        private DataCollection<ItemData> _cardCollection;

        public Action<string> OnPopulated = delegate { };

        protected override void Populate(List<string> fields, ScriptableObject @object, int IndexOf)
        {
            _billCollection ??= new(BillCollectionKey);
            _effectCollection ??= new(EffectCollectionKey);
            _explanationCollection ??= new(ExplanationCollectionKey);
            _cardCollection ??= new(CardCollectionKey);

            string log = string.Empty;
            ArtifactData artifactData = @object as ArtifactData;
            artifactData.MasterText = fields[3];
            artifactData.Level = IntParse(fields[4]);
            artifactData.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Join(_iconPath, "Artifacts=" + fields[2].Trim() + ".png"));
            if (artifactData.Icon == null)
            {
                log += $" - Icon: {Path.Join(_iconPath, "Artifacts=" + fields[2].Trim() + ".png")}\n";
            }
            artifactData.UpgradeFor = AssetDatabase.LoadAssetAtPath<ArtifactData>(Path.Join(DATA_PATH, fields[5] + ".asset"));
            ParseRequirements(artifactData, fields[6]);

            int index = 8;
            artifactData.Effects.Clear();
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
                artifactData.Effects.Add(newEffect);
            }
            var explanations = fields[index++];
            ParseExplanations(artifactData, explanations);
            artifactData.Description = fields[index++];

            artifactData.Price = IntParse(fields[index++]);
            artifactData.FollowersRequirement = IntParse(fields[index++]);
            artifactData.FollowerBonus = IntParse(fields[index++]);
            artifactData.BonusItems = ParseCards(fields[index++], out string bonusLogs);
            artifactData.RemovedCards = ParseCards(fields[index++], out string removedLogs);
            if (!string.IsNullOrEmpty(bonusLogs))
                log += bonusLogs;
            if (!string.IsNullOrEmpty(removedLogs))
                log += removedLogs;
            artifactData.Bill = _billCollection.GetFromName(fields[index++]);
            OnPopulated?.Invoke(log);
        }

        private List<ItemData> ParseCards(string field, out string logs)
        {
            logs = string.Empty;
            List<ItemData> cards = new();
            if (string.IsNullOrEmpty(field))
                return cards;

            foreach (var cardName in field.Split(','))
            {
                if (string.IsNullOrEmpty(cardName))
                    continue;
                var card = _cardCollection.GetFromName(cardName.Trim());
                if (card != null)
                {
                    cards.Add(card);
                }
                else
                {
                    logs += $" - Card: '{cardName}'";
                }
            }
            return cards;
        }

        private void ParseRequirements(ArtifactData artifactData, string requirements)
        {
            artifactData.Requirements.Clear();
            foreach (var requirementName in requirements.Split(','))
            {
                if (string.IsNullOrEmpty(requirementName))
                    continue;
                var requirement = AssetDatabase.LoadAssetAtPath<ArtifactData>(Path.Join(DATA_PATH, requirementName.Trim() + ".asset"));
                if (requirement != null)
                {
                    artifactData.Requirements.Add(requirement);
                }
                else
                {
                    Debug.LogWarning($"Requirement '{requirementName}' not found for artifact '{artifactData.name}'.");
                }
            }
        }

        private void ParseExplanations(ArtifactData data, string explanations)
        {
            data.Explanations.Clear();
            foreach (var explanationName in explanations.Split(','))
            {
                if (string.IsNullOrEmpty(explanationName))
                    continue;
                _explanationCollection ??= new(ExplanationCollectionKey);
                var explanation = _explanationCollection.GetFromName(explanationName.Trim());
                if (explanation != null)
                {
                    data.Explanations.Add(explanation);
                }
                else
                {
                    Debug.LogWarning($"Explanation '{explanationName}' not found for item '{data.name}'.");
                }
            }
        }
    }


}

#endif