#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Quackery
{

    public static class DataImportMenu
    {
        [MenuItem("Quackery/Import All Data")]
        public static async void ImportAllData()
        {
            var importer = AssetDatabase.LoadAssetAtPath<AllDataImporter>("Assets/_Content/ScriptableObjects/Importers/AllDataImporter.asset");
            if (importer != null)
            {
                await importer.Import();
                Debug.Log("All data imported successfully.");
            }
            else
            {
                Debug.LogError("AllDataImporter asset not found.");
            }
        }
    }
}
#endif