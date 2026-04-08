using System;
using System.Collections.Generic;
using Core.Enums;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "SaveData", menuName = "GameData/SaveData")]
    public class SaveData : ScriptableObject
    {
        [Serializable]
        public struct InventoryKeyEntry
        {
            public TileType tileType;
            public string saveKey;
        }

        [Header("Inventory Save Keys")]
        [SerializeField] private List<InventoryKeyEntry> inventoryKeys = new();

        private Dictionary<TileType, string> _keyMap;

        public string GetInventoryKey(TileType type)
        {
            BuildMapIfNeeded();
            return _keyMap.TryGetValue(type, out var key) ? key : $"inventory_{type}";
        }

        public IEnumerable<TileType> GetAllInventoryTypes()
        {
            BuildMapIfNeeded();
            return _keyMap.Keys;
        }

        public void ClearAllSaveData()
        {
            BuildMapIfNeeded();

            foreach (var key in _keyMap.Values)
                PlayerPrefs.DeleteKey(key);

            PlayerPrefs.Save();
            Debug.Log("[SaveData] All inventory save data cleared.");
        }

        private void BuildMapIfNeeded()
        {
            if (_keyMap != null) return;

            _keyMap = new Dictionary<TileType, string>(inventoryKeys.Count);
            foreach (var entry in inventoryKeys)
            {
                if (entry.tileType == TileType.None || string.IsNullOrWhiteSpace(entry.saveKey))
                    continue;

                if (!_keyMap.TryAdd(entry.tileType, entry.saveKey))
                    Debug.LogWarning($"[SaveData] Duplicate entry for TileType '{entry.tileType}' – skipping.", this);
            }
        }

#if UNITY_EDITOR
        private void OnValidate() => _keyMap = null;
#endif
    }
}

