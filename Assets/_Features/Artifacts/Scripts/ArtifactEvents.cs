using System;

namespace Quackery.Artifacts
{
    public static class ArtifactEvents
    {
        public static Action<ArtifactData> OnArtifactAdded = delegate { };

        public static Action<ArtifactData> OnArtifactOut = delegate { };
        public static Action OnArtifactPack = delegate { };
    }
}
