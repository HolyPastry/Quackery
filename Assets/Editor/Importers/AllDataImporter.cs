#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bakery.GoogleSheet;
using UnityEditor;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "AllDataImporter", menuName = "Quackery/GoogleSheet/AllDataImporter")]
    public class AllDataImporter : ScriptableObject
    {
        [SerializeField] private ExplanationImporter explanationImporters;
        [SerializeField] private CardImporter cardImporters;
        [SerializeField] private ArtifactImporter artifactImporters;
        [SerializeField] private DeckImporter deckImporters;

        public async Task Import()
        {
            await explanationImporters.Import();
            await cardImporters.Import();
            await artifactImporters.Import();
            await deckImporters.ImportDecks();
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