using Service;
using UnityEngine;

namespace Core.Pool
{
    public interface IPoolService : IService
    {
        public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component;
        public T Get<T>(T prefab, Transform parent) where T : Component;
        public void Return<T>(T prefab, T instance) where T : Component;
        public void Prewarm<T>(T prefab, int count, Transform parent = null) where T : Component;
    }
}

