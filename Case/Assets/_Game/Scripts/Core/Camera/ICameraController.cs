using UnityEngine;

namespace Core.Camera
{
    public interface ICameraController
    {
        public void SetFollowTarget(Transform target);
        public Vector3 ScreenToWorldPoint(Vector3 screenPosition);
    }
}