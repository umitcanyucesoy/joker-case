using Unity.Cinemachine;
using UnityEngine;

namespace Core.Camera
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        
        public void SetFollowTarget(Transform target)
        {
            cinemachineCamera.Follow = target;
            cinemachineCamera.LookAt = target;
        }
    }
}