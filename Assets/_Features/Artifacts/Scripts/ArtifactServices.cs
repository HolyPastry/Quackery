
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Artifacts
{
    public static class ArtifactServices
    {
        public static Action<ArtifactData> Add = delegate { };

        internal static Func<WaitUntil> WaitUntilReady = () => new WaitUntil(() => true);

        internal static Func<List<ArtifactData>> GetAllArtifacts = () => new();

    }
}
