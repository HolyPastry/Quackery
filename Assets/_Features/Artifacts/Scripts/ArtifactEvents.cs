using System;

namespace Quackery.Artifacts
{
    public static class ArtifactEvents
    {
        public static Action<ArtifactData> OnArtifactAdded = delegate { };
    }
}
