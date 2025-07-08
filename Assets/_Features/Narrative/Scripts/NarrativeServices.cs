

using System;
using System.Collections.Generic;

namespace Quackery.Narrative
{
    public static class NarrativeServices
    {
        public static Action<NarrativeData> AddNarrative = delegate { };
        public static Action<NarrativeData> ArchiveNarrative = delegate { };
        public static Func<List<NarrativeData>> GetInProgressNarratives = () => new();

    }
}
