using System.Collections.Generic;
using Core.Enums;
using UnityEngine;

namespace Core.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<TileType, int> _items = new();

        public int GetCount(TileType type)
        {
            return _items.GetValueOrDefault(type, 0);
        }

        public void AddItem(TileType type, int count)
        {
            if (type == TileType.None || count <= 0) return;

            if (_items.ContainsKey(type))
                _items[type] += count;
            else
                _items[type] = count;
        }

        public void RemoveItem(TileType type, int count)
        {
            if (!_items.ContainsKey(type)) return;

            _items[type] = Mathf.Max(0, _items[type] - count);

            if (_items[type] == 0)
                _items.Remove(type);
        }
    }
}

