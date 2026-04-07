using System;
using UnityEngine;

namespace Core.Dice
{
    [CreateAssetMenu(fileName = "DiceData", menuName = "GameData/DiceData")]
    public class DiceData : ScriptableObject
    {
        [Serializable]
        public struct FaceRotation
        {
            public int faceValue;
            public Vector3 eulerAngles;
        }

        [Header("Dice Prefab")]
        public DiceBehaviour dicePrefab;

        [Header("Face Rotations")]
        public FaceRotation[] faceRotations = 
        {
            new() { faceValue = 1, eulerAngles = new Vector3(-90f, 0f, 0f) },
            new() { faceValue = 2, eulerAngles = new Vector3(0f, 0f, 90f) },
            new() { faceValue = 3, eulerAngles = new Vector3(0f, 0f, 0f) },
            new() { faceValue = 4, eulerAngles = new Vector3(180f, 0f, 0f) },
            new() { faceValue = 5, eulerAngles = new Vector3(0f, 0f, -90f) },
            new() { faceValue = 6, eulerAngles = new Vector3(90f, 0f, 0f) }
        };

        [Header("Throw Settings")]
        public float throwForce = 8f;
        
        [Header("Roll Animation")]
        public int rollCount = 2;
        public float rollDuration = 0.6f;
        
        [Header("Timing")]
        public float delayBetweenDice = 0.15f;
        public float snapDuration = 0.1f;
        
        public Quaternion GetRotationForFace(int faceValue)
        {
            foreach (var face in faceRotations)
            {
                if (face.faceValue == faceValue)
                    return Quaternion.Euler(face.eulerAngles);
            }
            
            Debug.LogWarning($"[DiceData] Face {faceValue} not found, returning identity.");
            return Quaternion.identity;
        }
    }
}



