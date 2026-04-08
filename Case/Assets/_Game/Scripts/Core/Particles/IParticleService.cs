using Core.Enums;
using Service;
using UnityEngine;

namespace Core.Particles
{

    public interface IParticleService : IService
    {
        public void Play(ParticleType type, Vector3 position, Quaternion rotation = default);
        public void Prewarm();
    }
}

