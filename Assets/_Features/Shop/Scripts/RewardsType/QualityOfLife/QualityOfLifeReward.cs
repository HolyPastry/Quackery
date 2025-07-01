using Quackery.QualityOfLife;
using UnityEngine;

namespace Quackery.Shops
{
    [CreateAssetMenu(fileName = "QualityOfLifeReward", menuName = "Quackery/Shop/Quality Of Life Reward")]
    public class QualityOfLifeReward : ShopReward
    {
        private QualityOfLifeData _qualityOfLifeData;

        public QualityOfLifeData QualityOfLifeData
        {
            get
            {
                if (_qualityOfLifeData == null)
                {
                    _qualityOfLifeData = QualityOfLifeServices.GetRandomSuitable();
                }
                return _qualityOfLifeData;
            }
        }
        public override int Price => QualityOfLifeData.Price;
        public override bool IsSubscription => true;
        public override string Title => QualityOfLifeData.Title;
        public override string Description => QualityOfLifeData.Description;

        public override void ApplyReward()
        {
            QualityOfLifeServices.Acquire(QualityOfLifeData);
        }
    }
}
