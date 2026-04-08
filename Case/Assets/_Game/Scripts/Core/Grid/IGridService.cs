using Core.Data;
using Service;
using UnityEngine;

namespace Core.Grid
{
    public interface IGridService : IService
    {
        int Columns { get; }
        int Rows { get; }
        void BuildGrid(MapData mapData, Transform tileRoot);
        bool TryGetTileWorldPosition(Vector2Int coord, out Vector3 worldPosition);
        bool TryGetTile(Vector2Int coord, out Tile tile);
        void SetTypeRegistry(TileTypeRegistry registry);
    }
}