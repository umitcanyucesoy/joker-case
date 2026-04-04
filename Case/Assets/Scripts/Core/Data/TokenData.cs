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

        [Header("Token Value Settings")] 
        public float moveSpeed = 5f;
        public float heightOffset = 0.5f;
    }
}