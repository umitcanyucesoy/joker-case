using Core.Data;
using Service;

namespace Core.Grid
{
    public interface IGridService : IService
    {
        public void BuildGrid(MapData mapData);
    }
}