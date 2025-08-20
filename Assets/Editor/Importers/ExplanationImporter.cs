
#if UNITY_EDITOR
using System.Collections.Generic;
using Bakery.GoogleSheet;
using Holypastry.Bakery;
using UnityEngine;

namespace Quackery
{

    [CreateAssetMenu(fileName = "ExplanationImporter", menuName = "Quackery/GoogleSheet/ExplanationImporter")]
    public class ExplanationImporter : DataImporter<Explanation>
    {

        protected override void Populate(List<string> fields, ScriptableObject so, int IndexOf)
        {
            Explanation explanation = so as Explanation;

            explanation.Name = CapitalizeFirstLetterOnly(fields[0]);
            explanation.Definition = fields[1];
        }





    }


}

#endif