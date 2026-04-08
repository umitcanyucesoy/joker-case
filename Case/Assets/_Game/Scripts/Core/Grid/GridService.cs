using System.Collections.Generic;
using Core.Data;
using Core.Enums;
using Core.Pool;
using Service;
using UnityEngine;

namespace Core.Grid
{
    public class GridService : IGridService
    {
        private readonly Dictionary<Vector2Int, Tile> _tiles = new();
        private TileTypeRegistry _typeRegistry;
        private IPoolService _poolService;
        private Tile _tilePrefab;
        private float _tileSpacing;

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public void BuildGrid(MapData mapData, Transform tileRoot)
        {
            ClearGrid();
            
            _poolService = ServiceLocator.Get<IPoolService>();
            _tilePrefab = mapData.TilePrefab;

            var map = mapData.ParseGrid();

            if (map == null)
            {
                Debug.LogError("[GridService] Failed to parse grid data.");
                return;
            }

            Rows = map.rows;
            Columns = map.columns;
            _tileSpacing = mapData.TileSpacing;

            foreach (var cellData in map.cells)
            {
                var worldPosition = new Vector3(
                    -cellData.x * _tileSpacing,
                    0f,
                    cellData.y * _tileSpacing
                );

                var tile = _poolService.Get(_tilePrefab, worldPosition, Quaternion.identity, tileRoot);
                
                TileTypeData typeData = null;
                if (cellData.tileType != TileType.None)
                    _typeRegistry.TryGetType(cellData.tileType, out typeData);
                

                tile.Init(cellData, typeData);

                var coord = new Vector2Int(cellData.x, cellData.y);
                _tiles[coord] = tile;
            }

            Debug.Log($"[GridService] Grid built: {Columns}x{Rows}, total tiles: {_tiles.Count}");
            AssignTileNumbers();
        }
        
        public void SetTypeRegistry(TileTypeRegistry registry)
        {
            _typeRegistry = registry;
            _typeRegistry.Initialize();
        }

        private void AssignTileNumbers()
        {
            var number = 1;

            for (var row = 0; row < Rows; row++)
            {
                var isEvenRow = row % 2 == 0;

                if (isEvenRow)
                {
                    for (var col = 0; col < Columns; col++)
                    {
                        if (_tiles.TryGetValue(new Vector2Int(col, row), out var tile))
                            tile.SetTileNumber(number++);
                    }
                }
                else
                {
                    for (var col = Columns - 1; col >= 0; col--)
                    {
                        if (_tiles.TryGetValue(new Vector2Int(col, row), out var tile))
                            tile.SetTileNumber(number++);
                    }
                }
            }
        }

        public bool TryGetTileWorldPosition(Vector2Int coord, out Vector3 worldPosition)
        {
            if (_tiles.TryGetValue(coord, out var tile))
            {
                worldPosition = tile.transform.position;
                return true;
            }

            worldPosition = Vector3.zero;
            return false;
        }

        public bool TryGetTile(Vector2Int coord, out Tile tile)
        {
            return _tiles.TryGetValue(coord, out tile);
        }

        private void ClearGrid()
        {
            if (_poolService != null && _tilePrefab)
            {
                foreach (var kvp in _tiles)
                {
                    if (kvp.Value && kvp.Value.gameObject)
                        _poolService.Return(_tilePrefab, kvp.Value);
                }
            }
            else
            {
                foreach (var kvp in _tiles)
                {
                    if (kvp.Value && kvp.Value.gameObject)
                        Object.Destroy(kvp.Value.gameObject);
                }
            }

            _tiles.Clear();
            Rows = 0;
            Columns = 0;
        }
    }
}
