
using KBCore.Refs;
using TMPro;
using UnityEngine;


namespace Quackery
{
    public class VersionText : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private TextMeshProUGUI _versionText;

        void Start()
        {
            _versionText.text = $"{Application.version}";
        }
    }
}
