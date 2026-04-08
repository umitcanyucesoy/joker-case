using System.Collections.Generic;
using UnityEngine;

namespace Core.Pool
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<int, Queue<GameObject>> _pools = new();
        private readonly Dictionary<int, Transform> _poolParents = new();
        private Transform _poolRoot;

        private Transform PoolRoot
        {
            get
            {
                if (!_poolRoot)
                {
                    var go = new GameObject("[PoolService]");
                    Object.DontDestroyOnLoad(go);
                    _poolRoot = go.transform;
                }
                
                return _poolRoot;
            }
        }

        public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var go = GetInternal(prefab.gameObject, position, rotation, parent);
            return go.GetComponent<T>();
        }

        public T Get<T>(T prefab, Transform parent) where T : Component
        {
            var go = GetInternal(prefab.gameObject, Vector3.zero, Quaternion.identity, parent);
            go.transform.localScale = Vector3.one;
            return go.GetComponent<T>();
        }

        public void Return<T>(T prefab, T instance) where T : Component
        {
            ReturnInternal(prefab.gameObject, instance.gameObject);
        }

        public void Return(GameObject prefab, GameObject instance)
        {
            ReturnInternal(prefab, instance);
        }

        public void Prewarm<T>(T prefab, int count, Transform parent = null) where T : Component
        {
            var key = prefab.gameObject.GetInstanceID();
            EnsurePool(key, prefab.name);

            for (int i = 0; i < count; i++)
            {
                var instance = Object.Instantiate(prefab, GetPoolParent(key));
                instance.gameObject.SetActive(false);
                _pools[key].Enqueue(instance.gameObject);
            }
        }

        private GameObject GetInternal(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var key = prefab.GetInstanceID();
            EnsurePool(key, prefab.name);

            GameObject instance;

            if (_pools[key].Count > 0)
            {
                instance = _pools[key].Dequeue();

                var t = instance.transform;
                if (!Equals(t.parent, parent)) t.SetParent(parent, worldPositionStays: false);

                t.SetPositionAndRotation(position, rotation);
                instance.SetActive(true);
            }
            else
            {
                instance = Object.Instantiate(prefab, position, rotation, parent);
            }

            return instance;
        }

        private void ReturnInternal(GameObject prefab, GameObject instance)
        {
            var key = prefab.GetInstanceID();
            EnsurePool(key, prefab.name);

            instance.SetActive(false);
            _pools[key].Enqueue(instance);
        }

        private void EnsurePool(int key, string prefabName)
        {
            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<GameObject>();
                
                var parentGo = new GameObject($"Pool_{prefabName}");
                parentGo.transform.SetParent(PoolRoot);
                _poolParents[key] = parentGo.transform;
            }
        }

        private Transform GetPoolParent(int key)
        {
            return _poolParents.TryGetValue(key, out var parent) ? parent : PoolRoot;
        }
    }
}

