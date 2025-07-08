using System;

namespace Quackery.Artifacts
{
    public static class ArtifactEvents
    {
        public static Action<ArtifactData> OnArtifactAdded = delegate { };
        public static Action<ArtifactData> OnArtifactUpdated = delegate { };
    }
}
