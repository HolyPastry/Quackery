using Holypastry.Bakery;
using Quackery.TetrisBill;
using UnityEngine;


namespace Quackery.Bills
{
    [CreateAssetMenu(fileName = "BillData", menuName = "Quackery/Bills/BillData", order = 1)]
    public class BillData : ContentTag
    {

        public Sprite Icon;

        public float Chance;
        // [SerializeField] private int _startPrice;
        [SerializeField] private TetrisBlock _shape;

        // public virtual int Price => _startPrice;

        public TetrisBlock BlockPrefab
        {
            get => _shape;
            set => _shape = value;
        }



        public int PaymentInterval;
    }
}
