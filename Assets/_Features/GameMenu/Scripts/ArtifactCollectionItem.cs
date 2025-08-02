using System.Collections;
using System.Collections.Generic;
using Quackery.Artifacts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quackery.GameMenu
{
    public class ArtifactCollectionItem : MonoBehaviour, ITooltipTarget, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _description;

        private ArtifactData _artifactData;

        public List<Explanation> Explanations => _artifactData.Explanations;

        public RectTransform RectTransform => transform as RectTransform;

        public void Initialize(ArtifactData artifactData)
        {
            _artifactData = artifactData;
            _name.text = artifactData.MasterText;
            _icon.sprite = artifactData.Icon;
            _description.text = artifactData.Description;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Tooltips.ShowTooltipRequest(this);
        }
    }
}
