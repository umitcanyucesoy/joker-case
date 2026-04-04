using Core.Data;
using UnityEngine;

namespace Core.Grid
{
    public class Tile : MonoBehaviour
    {
        public int x;
        public int y;
        
        public Vector2Int Coordinates => new(x, y);

        public void Init(GridCellData cellData)
        {
            x = cellData.x;
            y = cellData.y;
            
            gameObject.name = $"Tile ({x}, {y})";
        }
    }
}