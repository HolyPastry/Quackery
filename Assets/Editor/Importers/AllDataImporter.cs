#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Quackery.Effects;
using UnityEditor;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "AllDataImporter", menuName = "Quackery/GoogleSheet/AllDataImporter")]
    public class AllDataImporter : ScriptableObject
    {
        [Header("Importers")]
        [SerializeField] private ExplanationImporter explanationImporters;
        [SerializeField] private CardImporter cardImporters;
        [SerializeField] private ArtifactImporter artifactImporters;
        [SerializeField] private DeckImporter deckImporters;
        [SerializeField] private BillImporter billImporters;
        [SerializeField] private StatusImporter statusImporter;

        [SerializeField] private string _spriteTexturePath = "Assets/_Content/2DArt/Sprites/TextSpriteTextureAtlas.png";


        public async Task Import()
        {
            string log = "";

            statusImporter.OnPopulated += (message) =>
            {
                if (!string.IsNullOrEmpty(message)) log += message;
            };

            cardImporters.OnPopulated += (message) =>
            {
                if (!string.IsNullOrEmpty(message)) log += message;
            };

            artifactImporters.OnPopulated += (message) =>
            {
                if (!string.IsNullOrEmpty(message)) log += message;
            };

            billImporters.OnPopulated += (message) =>
            {
                if (!string.IsNullOrEmpty(message)) log += message;
            };

            await explanationImporters.Import();
            log += "Status Art:\n";
            await statusImporter.Import();
            log += "Card Art:\n";
            await cardImporters.Import();
            log += "\n\nBill Art:\n";
            await billImporters.Import();
            log += "\n\nArtifact Art:\n";
            await artifactImporters.Import();
            await deckImporters.ImportDecks();

            log += "\n Statuses Icons:\n";
            // CheckStatuses(out string logs);
            // if (!string.IsNullOrEmpty(logs))
            //     log += logs;
            Debug.Log(log);
        }

        // private void CheckStatuses(out string logs)
        // {
        //     logs = string.Empty;


        //     DirectoryInfo dir = new DirectoryInfo(_statusesDataPath);
        //     if (!dir.Exists)
        //     {
        //         logs += $"Statuses data path '{_statusesDataPath}' does not exist.\n";
        //         return;
        //     }

        //     List<string> statusNames = new();
        //     var fileInfos = dir.GetFiles("*.asset", SearchOption.AllDirectories);

        //     foreach (var fileInfo in fileInfos)
        //     {
        //         string unityPath = Regex.Replace(fileInfo.FullName, @".*Assets/", "Assets/");
        //         EffectData effect = AssetDatabase.LoadAssetAtPath<EffectData>(unityPath);
        //         if (effect == null)
        //         {
        //             logs += $"EffectData not found at {unityPath}.\n";
        //             continue;
        //         }
        //         statusNames.Add(effect.name);
        //         if (!effect.Tags.Contains(EnumEffectTag.Status))
        //         {
        //             logs += $"EffectData {effect.name} is not a status.\n";
        //             continue;
        //         }
        //         // if (effect.Icon == null)
        //         // {
        //         //     if (!UpdateIcon(effect, out string iconLog))
        //         //     {
        //         //         logs += iconLog;
        //         //         continue;
        //         //     }

        //         // }
        //         // string path = AssetDatabase.GetAssetPath(effect.Icon);
        //         // if (!path.Contains($"Icon={effect.name}"))
        //         // {
        //         //     if (!UpdateIcon(effect, out string iconLog))
        //         //     {
        //         //         effect.Icon = null; // Reset icon if it cannot be updated
        //         //         EditorUtility.SetDirty(effect);
        //         //         logs += iconLog;
        //         //         continue;
        //         //     }
        //         // }
        //     }
        //     AssetDatabase.SaveAssets();
        //     AssetDatabase.Refresh();

        //     CheckSpriteAtlast(statusNames, out string atlasLog);
        //     if (!string.IsNullOrEmpty(atlasLog))
        //     {
        //         logs += "\nSprite Atlas Issues:\n";
        //         logs += atlasLog;
        //     }
        // }

        private void CheckSpriteAtlast(List<string> statusNames, out string atlasLog)
        {
            List<Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(_spriteTexturePath)
                            .OfType<Sprite>().ToList();
            atlasLog = string.Empty;
            foreach (var statusName in statusNames)
            {
                if (!sprites.Any(s => s.name == statusName))
                    atlasLog += $" - Sprite: {statusName}\n";
            }
        }




    }


    [CustomEditor(typeof(AllDataImporter))]
    public class AllDataImporterEditor : Editor
    {
        public override async void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Import"))
            {
                await ((AllDataImporter)target).Import();
                Debug.Log("Imported");
            }
        }
    }
}
#endif