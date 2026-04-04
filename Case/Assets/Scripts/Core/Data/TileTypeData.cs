using Core.Enums;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "TileType_", menuName = "GameData/TileType", order = 0)]
    public class TileTypeData : ScriptableObject
    {
        [Header("Visual Settings")] 
        public TileType tileType;
        public Sprite icon;
    }
}