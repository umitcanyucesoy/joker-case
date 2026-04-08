using Service;

namespace Save
{
    public interface ISaveService : IService
    {
        public void SaveInt(string key, int value);
        public int LoadInt(string key, int defaultValue = 0);
        public void Save();
    }
}

