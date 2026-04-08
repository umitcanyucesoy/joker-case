using System;
using System.Collections.Generic;
using Core.Enums;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "GameData/SoundData")]
    public class SoundData : ScriptableObject
    {
        [Serializable]
        public struct SoundEntry
        {
            public SoundType type;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
            [Range(0f, 0.3f)] public float pitchVariation;
        }

        [Header("Sound Effects")]
        [SerializeField] private List<SoundEntry> sounds = new();

        private Dictionary<SoundType, SoundEntry> _map;

        public bool TryGetEntry(SoundType type, out SoundEntry entry)
        {
            BuildMapIfNeeded();
            return _map.TryGetValue(type, out entry);
        }

        public IEnumerable<SoundEntry> GetAll()
        {
            BuildMapIfNeeded();
            return _map.Values;
        }

        private void BuildMapIfNeeded()
        {
            if (_map != null) return;

            _map = new Dictionary<SoundType, SoundEntry>(sounds.Count);
            foreach (var entry in sounds)
            {
                if (entry.type == SoundType.None || entry.clip == null) continue;

                if (!_map.TryAdd(entry.type, entry))
                    Debug.LogWarning($"[SoundData] Duplicate entry for '{entry.type}' – skipping.", this);
            }
        }

#if UNITY_EDITOR
        private void OnValidate() => _map = null;
#endif
    }
}

