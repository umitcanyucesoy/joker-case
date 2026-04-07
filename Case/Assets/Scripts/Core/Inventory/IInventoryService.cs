using System.Collections.Generic;
using Core.Enums;
using Service;

namespace Core.Inventory
{
    public interface IInventoryService : IService
    {
        public int GetCount(TileType type);
        public void AddItem(TileType type, int count);
    }
}

