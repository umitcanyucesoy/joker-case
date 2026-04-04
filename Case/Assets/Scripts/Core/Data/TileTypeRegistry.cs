using System.Collections.Generic;
using Core.Enums;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "TileTypeRegistry", menuName = "GameData/TileTypeRegistry")]
    public class TileTypeRegistry : ScriptableObject
    {
        [Tooltip("All available tile types")]
        [SerializeField] private List<TileTypeData> tileTypes = new();

        private Dictionary<TileType, TileTypeData> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<TileType, TileTypeData>();

            foreach (var data in tileTypes)
            {
                if (data == null) continue;

                if (!_lookup.TryAdd(data.tileType, data))
                    Debug.LogWarning($"[TileTypeRegistry] Duplicate: {data.tileType}");
            }
        }

        public bool TryGetType(TileType type, out TileTypeData data)
        {
            if (type == TileType.None)
            {
                data = null;
                return false;
            }

            return _lookup.TryGetValue(type, out data);
        }
    }
}