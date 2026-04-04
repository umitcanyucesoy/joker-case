using System;
using System.Collections.Generic;
using Core.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Grid
{
    public class GridService : IGridService
    {
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public void BuildGrid(MapData mapData)
        {
            ClearGrid();

            var map = mapData.ParseGrid();

            if (map == null)
            {
                Debug.LogError("[GridService] Failed to parse grid data.");
                return;
            }

            Rows = map.rows;
            Columns = map.columns;
            var tileSpacing = mapData.TileSpacing;

            var offsetX = (Columns - 1) * tileSpacing * 0.5f;
            var offsetY = (Rows - 1) * tileSpacing * 0.5f;

            foreach (var cellData in map.cells)
            {
                var worldPosition = new Vector3(
                    -cellData.x * tileSpacing,  
                    0f,                        
                    cellData.y * tileSpacing
                );

                var tile = Object.Instantiate(mapData.TilePrefab, worldPosition, Quaternion.identity);
                tile.Init(cellData);

                var coord = new Vector2Int(cellData.x, cellData.y);
                _tiles[coord] = tile;
            }

            Debug.Log($"[GridService] Grid built: {Columns}x{Rows}, total tiles: {_tiles.Count}");
        }
        
        private void ClearGrid()
        {
            foreach (var kvp in _tiles)
                if (kvp.Value && kvp.Value.gameObject)
                    Object.Destroy(kvp.Value.gameObject);

            _tiles.Clear();
            Rows = 0;
            Columns = 0;
        }
    }
}
