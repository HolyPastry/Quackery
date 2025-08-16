using System.Collections.Generic;
using Holypastry.Bakery;
using Quackery.Bills;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;




namespace Quackery.Artifacts
{


    [CreateAssetMenu(menuName = "Quackery/ArtifactData")]
    public class ArtifactData : ContentTag, IEffectCarrier
    {
        public Sprite Icon;

        public Sprite ShopBanner;

        [TextArea(3, 10)]
        public string Description;

        public ArtifactData UpgradeFor;

        public int Level;

        public List<ArtifactData> Requirements = new();
        public List<Explanation> Explanations = new();
        public bool IsUpgrade => UpgradeFor != null;

        public List<EffectData> EffectDataList => _effectDatas;

        [SerializeField] private List<EffectData> _effectDatas = new();

        public int Price = 0;

        public int FollowersRequirement = 0;
        public int FollowerBonus = 0;
        public BillData Bill = null;
        public List<ItemData> BonusItems = new();
        public List<ItemData> RemovedCards = new();
    }
}
