using Quackery.Artifacts;
using Quackery.Bills;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.QualityOfLife
{
    [CreateAssetMenu(fileName = "QualityOfLifeData", menuName = "Quackery/QualityOfLifeData", order = 1)]
    public class QualityOfLifeData : ScriptableObject
    {
        public string Title;

        [TextArea(3, 10)]
        public string Description;
        public Sprite ShopBanner;

        public int Price = 0;

        public int FollowersRequirement = 0;

        public int FollowerBonus = 0;

        public int RatingBonus = 0;

        public ItemData CardBonus = null;
        public ArtifactData ArtifactBonus = null;
        public BillData Bill = null;
    }
}
