using System.Collections;
using Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Dice
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceBehaviour : MonoBehaviour
    {
        public Rigidbody rb;
        
        private DiceData _diceData;
        private int _targetValue;
        private bool _isRolling;
        private float _stoppedTime;
        private float _stopVelocityThreshold;
        private float _stopDuration;

        public int CurrentFaceValue { get; private set; }
        public int InstanceId { get; private set; }

        public void Init(DiceData diceData, int instanceId)
        {
            _diceData = diceData;
            _stopVelocityThreshold = diceData.stopVelocityThreshold;
            _stopDuration = diceData.stopDuration;
            InstanceId = instanceId;
        }

        public void Throw(Vector3 direction, float force, float torqueForce, int targetValue)
        {
            _targetValue = Mathf.Clamp(targetValue, 1, 6);
            _isRolling = true;
            _stoppedTime = 0f;

            SetInitialRotationForTarget(_targetValue);

            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(direction.normalized * force, ForceMode.Impulse);

            var randomTorque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * torqueForce;
            
            rb.AddTorque(randomTorque, ForceMode.Impulse);
            StartCoroutine(WaitForStop());
        }

        private void SetInitialRotationForTarget(int targetValue)
        {
            var baseRotation = _diceData.GetRotationForFace(targetValue);
            
            var randomOffset = Quaternion.Euler(
               Random.Range(-10f, 10f),
               Random.Range(-10f, 10f),
               Random.Range(-10f, 10f)
            );
            
            transform.rotation = randomOffset * baseRotation;
        }

        private IEnumerator WaitForStop()
        {
            yield return new WaitForSeconds(0.5f);

            while (_isRolling)
            {
                var velocity = rb.linearVelocity.magnitude;
                var angularVelocity = rb.angularVelocity.magnitude;

                if (velocity < _stopVelocityThreshold && angularVelocity < _stopVelocityThreshold)
                {
                    _stoppedTime += Time.deltaTime;
                    
                    if (_stoppedTime >= _stopDuration)
                    {
                        SnapToTargetValue();
                        _isRolling = false;
                        CurrentFaceValue = _targetValue;
                        EventBus.Publish(new DiceStoppedEvent
                        {
                            InstanceId = InstanceId,
                            FaceValue = _targetValue
                        });
                    }
                }
                else
                {
                    _stoppedTime = 0f;
                }

                yield return null;
            }
        }

        private void SnapToTargetValue()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            var targetRotation = _diceData.GetRotationForFace(_targetValue);
            StartCoroutine(SmoothSnapRotation(targetRotation, 0.2f));
        }

        private IEnumerator SmoothSnapRotation(Quaternion targetRotation, float duration)
        {
            rb.isKinematic = true;
            
            var startRotation = transform.rotation;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / duration;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            transform.rotation = targetRotation;
        }

        public void ResetDice()
        {
            StopAllCoroutines();
            _isRolling = false;
            _stoppedTime = 0f;
            CurrentFaceValue = 0;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

