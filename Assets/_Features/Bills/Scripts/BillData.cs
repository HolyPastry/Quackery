using Holypastry.Bakery;
using UnityEngine;


namespace Quackery.Bills
{
    [CreateAssetMenu(fileName = "BillData", menuName = "Quackery/Bills/BillData", order = 1)]
    public class BillData : ContentTag
    {
        public Sprite Background;
        public Sprite Icon;
        [SerializeField] private int _startPrice;

        public virtual int Price => _startPrice;

        public int PaymentInterval;
    }
}
