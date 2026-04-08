using Core.Data;
using Core.Enums;
using TMPro;
using UnityEngine;

namespace Core.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TileTypeRenderer typeRenderer;
        [SerializeField] private TextMeshPro tileNumberText;
        
        public int x;
        public int y;
        public int TileNumber { get; private set; }
        public TileTypeData currentType;
        public int currentCount;

        public void Init(GridCellData cellData, TileTypeData typeData)
        {
            x = cellData.x;
            y = cellData.y;
            currentType = typeData;
            currentCount = cellData.count;
            
            gameObject.name = $"Tile ({x}, {y})";
            typeRenderer.Setup(typeData, currentCount);
        }

        public void SetTileNumber(int number)
        {
            TileNumber = number;
            tileNumberText.text = number.ToString();
        }
    }
}