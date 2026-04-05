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
            new() { faceValue = 1, eulerAngles = new Vector3(-90, 0, 0) },
            new() { faceValue = 2, eulerAngles = new Vector3(0, 0, 90) },
            new() { faceValue = 3, eulerAngles = new Vector3(0, 0, 0) },
            new() { faceValue = 4, eulerAngles = new Vector3(180, 0, 0) },
            new() { faceValue = 5, eulerAngles = new Vector3(0, 0, -90) },
            new() { faceValue = 6, eulerAngles = new Vector3(90, 0, 0) }
        };

        [Header("Throw Settings")]
        public float throwForce = 6f;
        public float torqueForce = 4f;
        [Range(0f, 0.5f)] public float upwardAngle = 0.1f;

        [Header("Timing")]
        public float delayBetweenDice = 0.2f;
        [Header("Detection Settings")]
        public float stopVelocityThreshold = 0.1f;
        public float stopDuration = 0.3f;

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




