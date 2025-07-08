#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace Bakery.GoogleSheet
{

    public abstract class DataImporter<T> : ScriptableObject where T : ScriptableObject
    {
        [SerializeField] protected GoogleSheetAPI _googleSheetAPI;
        [SerializeField] protected string DATA_PATH;
        [SerializeField] protected string SHEET_NAME;

        public event Action OnImported = delegate { };

        public void DeleteFolder()
        {
            //delete all assets in folder 
            string[] files = System.IO.Directory.GetFiles(DATA_PATH);
            foreach (string file in files)
            {
                AssetDatabase.DeleteAsset(file);
            }
            AssetDatabase.Refresh();
        }

        public virtual async Task Import()
        {

            EditorApplication.ExitPlaymode();
            Debug.ClearDeveloperConsole();

            List<List<string>> data = await _googleSheetAPI.Import(SHEET_NAME);

            for (int i = 1; i < data.Count; i++)
            {
                List<string> fields = data[i];
                string name = fields[0];
                if (string.IsNullOrEmpty(name))
                    continue;

                ScriptableObject so = GetScriptableObject(name);

                Populate(fields, so, i);
                EditorUtility.SetDirty(so);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            OnImported.Invoke();
        }

        private ScriptableObject GetScriptableObject(string name)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath($"{DATA_PATH}/{name}.asset", typeof(T));
            if (obj != null)
            {
                return (T)obj;
            }
            else
            {
                T so = CreateInstance(typeof(T).Name) as T;
                so.name = name;
                AssetDatabase.CreateAsset(so, $"{DATA_PATH}/{name}.asset");
                return so;
            }

        }

        protected abstract void Populate(List<string> fields, ScriptableObject @object, int IndexOf);
        public static int IntParse(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return 0;
            }
            return int.Parse(v);
        }
        public static bool IsFieldTrue(string field)
        {
            return field == "TRUE";
        }

        public static string CapitalizeFirstLetterOnly(string field)
        {
            return field[..1].ToUpper() + field[1..].ToLower();
        }


    }

}
#endif
