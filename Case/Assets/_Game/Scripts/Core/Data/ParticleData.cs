using System;
using System.Collections.Generic;
using Core.Enums;
using Core.Particles;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "ParticleData", menuName = "GameData/ParticleData")]
    public class ParticleData : ScriptableObject
    {
        [Serializable]
        public struct ParticleEntry
        {
            public ParticleType type;
            public ParticleBehaviour prefab;
            public int prewarmCount;
        }

        [Header("Particle Effects")]
        [SerializeField] private List<ParticleEntry> particles = new();

        private Dictionary<ParticleType, ParticleEntry> _map;

        public bool TryGetEntry(ParticleType type, out ParticleEntry entry)
        {
            BuildMapIfNeeded();
            return _map.TryGetValue(type, out entry);
        }
        
        public IEnumerable<ParticleEntry> GetAll()
        {
            BuildMapIfNeeded();
            return _map.Values;
        }

        private void BuildMapIfNeeded()
        {
            if (_map != null) return;

            _map = new Dictionary<ParticleType, ParticleEntry>(particles.Count);
            foreach (var entry in particles)
            {
                if (entry.type == ParticleType.None || entry.prefab == null) continue;

                if (!_map.TryAdd(entry.type, entry))
                    Debug.LogWarning($"[ParticleData] Duplicate entry for '{entry.type}' – skipping.", this);
            }
        }

#if UNITY_EDITOR
        private void OnValidate() => _map = null;
#endif
    }
}



