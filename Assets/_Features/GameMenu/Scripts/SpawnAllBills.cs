using UnityEngine;

namespace Quackery.GameMenu
{
    public class SpawnAllBills : MonoBehaviour
    {
        [SerializeField] private BillCollectionItem _billPrefab;

        void OnEnable()
        {
            SpawnAll();
        }

        void OnDisable()
        {
            DestroyAll();
        }

        private void SpawnAll()
        {
            var bills = BillServices.GetAllBills();
            foreach (var bill in bills)
            {
                var item = Instantiate(_billPrefab, transform);
                item.Initialize(bill);
            }
        }

        private void DestroyAll()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
