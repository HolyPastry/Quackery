using Holypastry.Bakery;
using UnityEngine;


namespace Quackery.Bills
{
    [CreateAssetMenu(fileName = "BillData", menuName = "Quackery/BillData", order = 1)]
    public class BillData : ContentTag
    {
        public Sprite Background;
        public Sprite Icon;
        public int StartPrice;


        public int PaymentInterval;
    }
}
