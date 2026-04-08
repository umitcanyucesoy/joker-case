using System.Collections.Generic;
using Core.Data;
using Core.Enums;
using Core.Pool;
using Event;
using UnityEngine;

namespace Core.Particles
{
    public class ParticleService : IParticleService
    {
        private readonly IPoolService _poolService;
        private readonly ParticleData _particleData;

        private readonly Dictionary<ParticleType, ParticleBehaviour> _prefabMap = new();

        public ParticleService(IPoolService poolService, ParticleData particleData)
        {
            _poolService  = poolService;
            _particleData = particleData;

            foreach (var entry in _particleData.GetAll())
                _prefabMap[entry.type] = entry.prefab;

            EventBus.Subscribe<ParticleCompletedEvent>(OnParticleCompleted);
        }

        public void Play(ParticleType type, Vector3 position, Quaternion rotation = default)
        {
            if (type == ParticleType.None) return;

            if (!_particleData.TryGetEntry(type, out var entry))
            {
                Debug.LogWarning($"[ParticleService] No ParticleData entry for type '{type}'.");
                return;
            }

            var instance = _poolService.Get(entry.prefab, position, rotation);
            instance.Play(type);
        }

        public void Prewarm()
        {
            foreach (var entry in _particleData.GetAll())
            {
                if (entry.prewarmCount > 0)
                    _poolService.Prewarm(entry.prefab, entry.prewarmCount);
            }
        }

        private void OnParticleCompleted(ParticleCompletedEvent evt)
        {
            if (!_prefabMap.TryGetValue(evt.ParticleType, out var prefab)) return;
            _poolService.Return(prefab, evt.Instance);
        }
    }
}

