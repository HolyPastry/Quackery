using System;
using System.Collections.Generic;

namespace Quackery.QualityOfLife
{
    public static class QualityOfLifeServices
    {
        public static Func<int, List<QualityOfLifeData>> GetSuitable = (num) => new();
        public static Action<QualityOfLifeData> Acquire = (data) => { };

        internal static Func<QualityOfLifeData> GetRandomSuitable = () => null;


    }
}
