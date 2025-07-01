using Quackery.QualityOfLife;
using UnityEngine;

namespace Quackery.Shops
{

    public class QualityOfLifeReward : ShopReward
    {

        public QualityOfLifeData QualityOfLifeData { get; private set; }

        public override int Price => QualityOfLifeData.Price;
        public override bool IsSubscription => true;
        public override string Title => QualityOfLifeData.Title;
        public override string Description => QualityOfLifeData.Description;

        public QualityOfLifeReward(QualityOfLifeData qualityOfLifeData)
        {
            QualityOfLifeData = qualityOfLifeData;
        }

        public override void ApplyReward()
        {
            QualityOfLifeServices.Acquire(QualityOfLifeData);
        }
    }
}
