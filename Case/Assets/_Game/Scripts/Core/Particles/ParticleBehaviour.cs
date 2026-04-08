using System.Collections;
using Core.Enums;
using Event;
using UnityEngine;

namespace Core.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleBehaviour : MonoBehaviour
    {
        public ParticleSystem ps;
        private Coroutine _waitRoutine;
        private ParticleType _particleType;
        
        public void Play(ParticleType type)
        {
            _particleType = type;

            if (_waitRoutine != null)
                StopCoroutine(_waitRoutine);

            ps.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();

            _waitRoutine = StartCoroutine(WaitForCompletion());
        }

        private IEnumerator WaitForCompletion()
        {
            yield return null;
            yield return new WaitUntil(() => !ps.IsAlive(withChildren: true));

            _waitRoutine = null;
            EventBus.Publish(new ParticleCompletedEvent
            {
                ParticleType = _particleType,
                Instance = this
            });
        }

        private void OnDisable()
        {
            if (_waitRoutine == null) return;
            StopCoroutine(_waitRoutine);
            _waitRoutine = null;
        }
    }
}
