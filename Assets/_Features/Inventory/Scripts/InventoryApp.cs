using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Quackery.Inventories
{
    public class InventoryApp : MonoBehaviour
    {

        [SerializeField] private ItemUI _itemUIPrefab;
        [SerializeField] private Transform _itemContainer;
        private bool _initialized;

        private readonly List<ItemUI> _itemUIs = new();

        void OnEnable()
        {
            if (_initialized)
                StartCoroutine(UpdateUIRoutine());
        }
        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator Start()
        {
            yield return InventoryServices.WaitUntilReady();
            StartCoroutine(UpdateUIRoutine());
            _initialized = true;
        }

        private IEnumerator UpdateUIRoutine()
        {

            while (true)
            {
                var items = InventoryServices.GetAllItems();
                foreach (var item in items)
                {
                    // Check if the item UI already exists
                    var existingItemUI = _itemUIs.Find(ui => ui.Item.Data == item.Data);
                    if (existingItemUI != null)
                    {
                        // Update existing UI
                        existingItemUI.SetItem(item);
                        continue;
                    }
                    else
                    {
                        var itemUI = Instantiate(_itemUIPrefab, _itemContainer);
                        itemUI.SetItem(item);
                        _itemUIs.Add(itemUI);
                    }
                }
                // Remove UIs for items that no longer exist
                for (int i = _itemUIs.Count - 1; i >= 0; i--)
                {
                    if (!InventoryServices.HasItem(_itemUIs[i].Item.Data))
                    {
                        Destroy(_itemUIs[i].gameObject);
                        _itemUIs.RemoveAt(i);
                    }
                }
                yield return new WaitForSeconds(1f); // Update every second
            }
        }

    }
}
