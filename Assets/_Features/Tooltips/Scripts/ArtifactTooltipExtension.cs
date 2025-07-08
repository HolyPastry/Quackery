using System;
using Quackery.Artifacts;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    public class ArtifactTooltipExtension : TooltipExtension
    {
        public override void SetTooltip(GameObject hoveredObject)
        {
            if (hoveredObject.TryGetComponent(out ArtifactUI artifactUI))
                AddTooltip(artifactUI.Artifact);

        }

        private void AddTooltip(ArtifactData artifact)
        {
            string description = artifact.Description;
            string title = artifact.MasterText;

            _tooltipManager.AddTooltip(title, description, new());
        }


    }
}
