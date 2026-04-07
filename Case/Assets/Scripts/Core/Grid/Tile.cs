using Core.Data;
using Core.Enums;
using UnityEngine;

namespace Core.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TileTypeRenderer typeRenderer;
        
        public int x;
        public int y;
        public TileTypeData currentType;
        public int currentCount;
        
        public Vector2Int Coordinates => new(x, y);

        public void Init(GridCellData cellData, TileTypeData typeData)
        {
            x = cellData.x;
            y = cellData.y;
            currentType = typeData;
            currentCount = cellData.count;
            
            gameObject.name = $"Tile ({x}, {y})";
            typeRenderer.Setup(typeData, currentCount);
        }
        
        public void SetType(TileTypeData typeData, int count)
        {
            currentType = typeData;
            currentCount = count;
            typeRenderer?.Setup(typeData, count);
        }
        
        public void UpdateCount(int newCount)
        {
            currentCount = newCount;
            typeRenderer?.UpdateCount(newCount);
        }
    }
}