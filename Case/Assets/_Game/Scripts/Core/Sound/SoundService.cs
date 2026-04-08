using Core.Data;
using Core.Enums;
using UnityEngine;

namespace Core.Sound
{
    public class SoundService : ISoundService
    {
        private readonly SoundData _soundData;
        private readonly AudioSource _source;

        public SoundService(SoundData soundData)
        {
            _soundData = soundData;

            var go = new GameObject("[SoundService]");
            Object.DontDestroyOnLoad(go);
            _source = go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        public void Play(SoundType type)
        {
            if (type == SoundType.None) return;

            if (!_soundData.TryGetEntry(type, out var entry))
            {
                Debug.LogWarning($"[SoundService] No SoundData entry for type '{type}'.");
                return;
            }

            _source.pitch = 1f + Random.Range(-entry.pitchVariation, entry.pitchVariation);
            _source.PlayOneShot(entry.clip, entry.volume);
        }
    }
}

