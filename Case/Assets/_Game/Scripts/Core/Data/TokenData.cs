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
        
        [Header("Lap Bounce Animation Settings")]
        public float lapBounceHeight = 2f;
        public float lapBounceDuration = 0.8f;
        public AnimationCurve lapBounceCurve = new(
            new Keyframe(0f, 0f, 0f, 1f),   
            new Keyframe(0.6f, 1f, 1f, 0f),    
            new Keyframe(0.85f, 0.2f, -4f, -4f),
            new Keyframe(1f, 0f, -0.5f, 0f)     
        );
        public AnimationCurve lapMoveCurve = new(
            new Keyframe(0f, 0f, 0f, 0.5f),    
            new Keyframe(0.6f, 0.3f, 0.5f, 2f), 
            new Keyframe(1f, 1f, 1.5f, 0f)      
        );
    }
}