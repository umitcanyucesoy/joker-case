using Unity.Cinemachine;
using UnityEngine;

namespace Core.Camera
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private UnityEngine.Camera renderCamera;
        
        [Header("Smooth Follow Settings")]
        [SerializeField] private Transform smoothFollowProxy;
        [SerializeField] private bool ignoreVerticalMovement = true;
        [SerializeField] private float verticalSmoothSpeed = 2f;
        
        private Transform _followTarget;
        private float _baseY;

        public Vector3 ScreenToWorldPoint(Vector3 screenPosition) => renderCamera.ScreenToWorldPoint(screenPosition);

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
            _baseY = target.position.y;
            
            if (ignoreVerticalMovement && smoothFollowProxy)
            {
                smoothFollowProxy.position = target.position;
                cinemachineCamera.Follow = smoothFollowProxy;
                cinemachineCamera.LookAt = smoothFollowProxy;
            }
            else
            {
                cinemachineCamera.Follow = target;
                cinemachineCamera.LookAt = target;
            }
        }

        private void LateUpdate()
        {
            if (!ignoreVerticalMovement || !_followTarget || !smoothFollowProxy) return;
            
            var targetPos = _followTarget.position;
            _baseY = Mathf.Lerp(_baseY, targetPos.y, verticalSmoothSpeed * Time.deltaTime);
            smoothFollowProxy.position = new Vector3(targetPos.x, _baseY, targetPos.z);
        }
    }
}