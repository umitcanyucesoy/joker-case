using System.Collections.Generic;
using Core.Data;
using Core.Enums;
using Save;
using UnityEngine;

namespace Core.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<TileType, int> _items = new();
        private readonly ISaveService _saveService;
        private readonly SaveData _saveData;

        public InventoryService(ISaveService saveService, SaveData saveData)
        {
            _saveService = saveService;
            _saveData    = saveData;
            LoadInventory();
        }
        
        public int GetCount(TileType type) => _items.GetValueOrDefault(type, 0);

        public void AddItem(TileType type, int count)
        {
            if (type == TileType.None || count <= 0) return;

            if (_items.ContainsKey(type))
                _items[type] += count;
            else
                _items[type] = count;

            SaveItem(type);
        }

        public void RemoveItem(TileType type, int count)
        {
            if (!_items.ContainsKey(type)) return;

            _items[type] = Mathf.Max(0, _items[type] - count);

            if (_items[type] == 0)
                _items.Remove(type);

            SaveItem(type);
        }

        private void LoadInventory()
        {
            foreach (var type in _saveData.GetAllInventoryTypes())
            {
                var key   = _saveData.GetInventoryKey(type);
                var count = _saveService.LoadInt(key, 0);

                if (count > 0)
                    _items[type] = count;
            }
        }

        private void SaveItem(TileType type)
        {
            var key   = _saveData.GetInventoryKey(type);
            var count = _items.GetValueOrDefault(type, 0);
            _saveService.SaveInt(key, count);
            _saveService.Save();
        }
    }
}

