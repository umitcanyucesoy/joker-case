using System;
using Core.Enums;

namespace Core.Data
{
    [Serializable]
    public class GridCellData
    {
        public int x;
        public int y;
        public TileType tileType; 
        public int count;
    }
}