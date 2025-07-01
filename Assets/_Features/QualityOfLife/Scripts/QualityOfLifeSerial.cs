using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;


namespace Quackery.QualityOfLife
{
    public class QualityOfLifeSerial : SerialData
    {
        public List<string> _qualityOfLifeDataNames = new();

        public void Add(QualityOfLifeData data)
            => _qualityOfLifeDataNames.AddUnique(data.name);

        public bool Contains(QualityOfLifeData data)
            => _qualityOfLifeDataNames.Contains(data.name);

    }
}
