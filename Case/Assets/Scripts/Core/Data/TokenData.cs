using Core.Tokens;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "Token_", menuName = "GameData/TokenData", order = 0)]
    public class TokenData : ScriptableObject
    {
        [Header("Token Visual Settings")]
        public string displayName;
        public Token prefab;
        public Sprite icon;

        [Header("Token Movement Settings")] 
        public float moveSpeed = 5f;
        public float heightOffset = 0.5f;
        
        [Header("Jump Animation Settings")]
        public float jumpHeight = 0.5f;
        public float jumpDuration = 0.3f;
        public AnimationCurve jumpCurve = new(
            new Keyframe(0f, 0f, 0f, 2f),
            new Keyframe(0.5f, 1f, 0f, 0f),
            new Keyframe(1f, 0f, -2f, 0f)
        );
        public AnimationCurve moveCurve = new(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(1f, 1f, 0f, 0f)
        );
        public float stepDelay = 0.05f;
    }
}