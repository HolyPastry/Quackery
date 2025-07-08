
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Bakery.GoogleSheet;
using Holypastry.Bakery;
using Quackery.Artifacts;
using Quackery.Effects;
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


        private DataCollection<EffectData> _effectCollection;
        private DataCollection<Explanation> _explanationCollection;
        protected override void Populate(List<string> fields, ScriptableObject @object, int IndexOf)
        {
            ArtifactData artifactData = @object as ArtifactData;
            artifactData.MasterText = fields[3];
            artifactData.Level = IntParse(fields[4]);
            artifactData.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Join(_iconPath, "Artifact=" + fields[2] + ".png"));
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
                    Debug.LogWarning($"Effect '{effectName}' not found in collection '{EffectCollectionKey}' for item '{artifactData.name}' at index {IndexOf}.");
                    continue;
                }
                var newEffect = new Effect(effectData)
                {
                    Value = effectValue
                };
                artifactData.Effects.Add(newEffect);
            }
            var explanations = fields[index++];
            ParseExplanations(artifactData, explanations);
            artifactData.Description = fields[index++];
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