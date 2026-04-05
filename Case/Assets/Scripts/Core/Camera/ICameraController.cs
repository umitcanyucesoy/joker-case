using UnityEngine;

namespace Core.Camera
{
    public interface ICameraController
    {
        public void SetFollowTarget(Transform target);
    }
}