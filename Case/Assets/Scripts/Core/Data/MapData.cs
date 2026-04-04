using System;
using System.Collections.Generic;
using Core.Grid;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "Level_", menuName = "GameData/Level Data")]
    public class MapData : ScriptableObject
    {
        [Serializable]
        public class GridMapData
        {
            public int rows;
            public int columns;
            public List<GridCellData> cells;
        }
        
        [Tooltip("JSON file containing the grid layout for this level.")]
        public TextAsset GridJson;
        [Tooltip("Prefab used for each tile in the grid.")]
        public Tile TilePrefab;
        [Tooltip("Spacing between tiles in world units.")]
        public float TileSpacing;

        public GridMapData ParseGrid()
        {
            if (!GridJson)
            {
                Debug.LogError($"[LevelData] GridJson is not assigned on {name}");
                return null;
            }
            
            return JsonUtility.FromJson<GridMapData>(GridJson.text);
        }
    }
}