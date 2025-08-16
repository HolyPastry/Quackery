using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery
{
    public static class Tempo
    {
        public const float Beat = 0.65f;

        public static YieldInstruction WaitForABeat => new WaitForSeconds(Beat);
    }
}
