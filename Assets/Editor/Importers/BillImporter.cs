
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Bakery.GoogleSheet;
using Quackery.Bills;
using Quackery.TetrisBill;
using UnityEditor;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "BillImporter", menuName = "Quackery/GoogleSheet/BillImporter")]
    public class BillImporter : DataImporter<BillData>
    {
        [SerializeField] private string _iconPath = "Assets/_Content/2DArt/Sprites/Bills";
        [SerializeField] private string _prefabPath = "Assets/_Content/Prefabs/BillShapes";

        public event Action<string> OnPopulated = delegate { };

        protected override void Populate(List<string> fields, ScriptableObject so, int IndexOf)
        {
            string log = string.Empty;
            BillData billData = so as BillData;
            billData.MasterText = fields[3];

            billData.Icon = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Join(_iconPath, "Bill=" + fields[2].Trim() + ".png"));
            if (billData.Icon == null)
            {
                log += $" - Icon: {Path.Join(_iconPath, "Bill=" + fields[2].Trim() + ".png")}\n";
            }

            billData.BlockPrefab = AssetDatabase.LoadAssetAtPath<TetrisBlock>(Path.Join(_prefabPath, fields[4] + ".prefab"));
            if (billData.BlockPrefab == null)
            {
                log += $" - Block Prefab: {Path.Join(_prefabPath, fields[4] + ".prefab")}\n";
            }
            billData.Chance = PercentParse(fields[5]);
            OnPopulated?.Invoke(log);
        }

        private float PercentParse(string percent)
        {
            if (string.IsNullOrEmpty(percent))
                return 0f;
            percent = percent.TrimEnd('%');
            return float.Parse(percent) / 100f;
        }
    }

}

#endif