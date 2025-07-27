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
            if (!hoveredObject.TryGetComponent(out ArtifactUI artifactUI)) return;

            //AddTooltip(artifactUI.Artifact);
            //_tooltipManager.AddTooltip(title, description, new());

        }




    }
}
