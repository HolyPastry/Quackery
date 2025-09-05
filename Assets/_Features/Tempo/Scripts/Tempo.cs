using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public static class Tempo
    {
        public const float WholeBeat = 0.65f;
        public const float HalfBeat = WholeBeat / 2;
        public const float QuarterBeat = WholeBeat / 4;
        public const float EighthBeat = WholeBeat / 8;

        public static YieldInstruction WaitForABeat => new WaitForSeconds(WholeBeat);

        public static YieldInstruction WaitForEighthBeat => new WaitForSeconds(EighthBeat);

        public static YieldInstruction WaitForHalfABeat => new WaitForSeconds(HalfBeat);
    }
}
