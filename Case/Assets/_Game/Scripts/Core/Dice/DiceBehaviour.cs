using System.Collections;
using Event;
using UnityEngine;

namespace Core.Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        
        private DiceData _diceData;
        private int _targetValue;
        private Quaternion _targetRotation;
        private Vector3 _rollAxis;

        private bool _groundHitFired;

        public int CurrentFaceValue { get; private set; }
        public int InstanceId { get; private set; }

        public void Init(DiceData diceData, int instanceId)
        {
            _diceData = diceData;
            InstanceId = instanceId;
            _groundHitFired = false;
        }

        public void Throw(Vector3 direction, int targetValue)
        {
            _targetValue = Mathf.Clamp(targetValue, 1, 6);
            _targetRotation = _diceData.GetRotationForFace(_targetValue);
            
            _rollAxis = Vector3.Cross(Vector3.up, direction).normalized;

            transform.rotation = _targetRotation;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(direction.normalized * _diceData.throwForce, ForceMode.Impulse);
            StartCoroutine(RollCoroutine());
        }

        private IEnumerator RollCoroutine()
        {
            var rollStartTime = Time.time;
            var targetTotalRotation = 360f * _diceData.rollCount;

            while (true)
            {
                var elapsed = Time.time - rollStartTime;
                var t = elapsed / _diceData.rollDuration;
                
                if (t >= 1f) break;
                
                var easedT = 1f - Mathf.Pow(1f - t, 3f);
                var currentAngle = easedT * targetTotalRotation;
                
                transform.rotation = Quaternion.AngleAxis(currentAngle, _rollAxis) * _targetRotation;
                yield return null;
            }

            transform.rotation = _targetRotation;

            while (rb.linearVelocity.magnitude > 0.1f)
                yield return null;
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            
            CurrentFaceValue = _targetValue;

            EventBus.Publish(new DiceStoppedEvent
            {
                InstanceId = InstanceId,
                FaceValue = _targetValue
            });
        }

        private void FixedUpdate()
        {
            if (!rb.isKinematic) rb.angularVelocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var contact = collision.GetContact(0).point;

            if (collision.gameObject.TryGetComponent<DiceBehaviour>(out _))
            {
                EventBus.Publish(new DiceCollisionEvent { ContactPoint = contact });
                return;
            }

            if (_groundHitFired) return;
            _groundHitFired = true;

            EventBus.Publish(new DiceGroundHitEvent { ContactPoint = contact });
        }

        public void ResetDice()
        {
            StopAllCoroutines();
            CurrentFaceValue = 0;
            
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }
    }
}










