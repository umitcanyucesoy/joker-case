using Core.Data;
using Service;
using UnityEngine;

namespace Core.Grid
{
    public interface IGridService : IService
    {
        public int Columns { get; }
        public int Rows { get; }
        public void BuildGrid(MapData mapData, Transform tileRoot);
        public bool TryGetTileWorldPosition(Vector2Int coord, out Vector3 worldPosition);
        public void SetTypeRegistry(TileTypeRegistry registry);
    }
}