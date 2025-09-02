
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
    [CreateAssetMenu(fileName = "StatusImporter", menuName = "Quackery/GoogleSheet/StatusImporter")]
    public class StatusImporter : DataImporter<Status>
    {

        [SerializeField] private string _iconPath = "Assets/_Content/2DArt/Sprites/Icons";

        public event Action<string> OnPopulated = delegate { };

        private DataCollection<Explanation> _explanations;

        protected override void Populate(List<string> fields, ScriptableObject @object, int IndexOf)
        {
            _explanations ??= new("Explanations");
            string log = string.Empty;
            Status status = @object as Status;
            status.Name = fields[0];
            status.Target = (EnumTarget)Enum.Parse(typeof(EnumTarget), fields[1]);
            if (!UpdateIcon(status, out string iconLog))
            {
                log += iconLog;
            }
            var explanation = _explanations.GetFromName(status.name);
            if (explanation == null)
                log += $"- Explanation: {status.name}\n";
            else
                status.Explanation = explanation;


            OnPopulated(log);
        }

        private bool UpdateIcon(Status status, out string iconLog)
        {
            iconLog = string.Empty;
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(_iconPath, $"Icon={status.name}.png"));
            if (icon != null)
            {
                status.Icon = icon;
                EditorUtility.SetDirty(status);
                return true;
            }

            iconLog = $" - Icon: {Path.Combine(_iconPath, $"Icon={status.name}.png")}\n";
            return false;

        }
    }



}

#endif