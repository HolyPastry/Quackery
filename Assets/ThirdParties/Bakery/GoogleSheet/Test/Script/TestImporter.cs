#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bakery.GoogleSheet
{
    [CreateAssetMenu(fileName = "TestImporter", menuName = "Bakery/GoogleSheet/TestImporter")]
    public class TestImporter : DataImporter<TestTemplate>
    {
        //return the name of the scriptable object
        protected override void Populate(List<string> fields, ScriptableObject so, int index)
        {

            TestTemplate testTemplate = so as TestTemplate;

            testTemplate.Title = CapitalizeFirstLetterOnly(fields[1]);
            testTemplate.Description = fields[2];


        }
    }


    [CustomEditor(typeof(TestImporter))]
    public class TestImporterEditor : Editor
    {
        public override async void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Import"))
            {
                await ((TestImporter)target).Import();
                Debug.Log("Imported");
            }
        }
    }
}


#endif