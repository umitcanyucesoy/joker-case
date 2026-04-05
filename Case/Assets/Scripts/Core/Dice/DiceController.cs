using System.Collections;
using System.Collections.Generic;
using Event;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Dice
{
    public class DiceController : MonoBehaviour, IDiceController
    {
        [Header("References")]
        [SerializeField] private DiceData diceData;
        [SerializeField] private CinemachineCamera mainCamera;

        [Header("Spawn Settings")]
        [SerializeField] private float groundHeight = 0.5f;
        [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0f, 1f);

        private readonly List<DiceBehaviour> _activeDice = new();
        private bool _isRolling;
        private int _diceStoppedCount;
        private int _totalResult;

        public bool IsRolling => _isRolling;

        private void OnEnable()
        {
            EventBus.Subscribe<DiceStoppedEvent>(OnDiceStopped);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DiceStoppedEvent>(OnDiceStopped);
        }

        public void ThrowDice(int dice1Value, int dice2Value)
        {
            if (_isRolling)
            {
                Debug.LogWarning("[DiceController] Dice are already rolling.");
                return;
            }

            StartCoroutine(ThrowDiceSequence(dice1Value, dice2Value));
        }

        private IEnumerator ThrowDiceSequence(int dice1Value, int dice2Value)
        {
            _isRolling = true;
            _diceStoppedCount = 0;
            _totalResult = 0;

            CleanupDice();
            
            var spawnBase = GetSpawnPosition();
            var dice1 = SpawnAndThrowDice(spawnBase, dice1Value, 0);
            _activeDice.Add(dice1);

            yield return new WaitForSeconds(diceData.delayBetweenDice);

            var dice2 = SpawnAndThrowDice(spawnBase, dice2Value, 1);
            _activeDice.Add(dice2);

            yield return new WaitUntil(() => _diceStoppedCount >= 2);

            EventBus.Publish(new DiceRollCompletedEvent
            {
                Dice1Value = dice1.CurrentFaceValue,
                Dice2Value = dice2.CurrentFaceValue,
                TotalValue = _totalResult
            });

            _isRolling = false;
        }

        private Vector3 GetSpawnPosition()
        {
            var camTransform = mainCamera.transform;
            var camPos = camTransform.position;
            
            var flatForward = camTransform.forward;
            flatForward.y = 0;
            flatForward.Normalize();
            
            var flatRight = camTransform.right;
            flatRight.y = 0;
            flatRight.Normalize();

            var spawnPos = new Vector3(camPos.x, groundHeight, camPos.z) + flatRight * spawnOffset.x + flatForward * spawnOffset.z;
            return spawnPos;
        }

        private DiceBehaviour SpawnAndThrowDice(Vector3 basePosition, int targetValue, int diceIndex)
        {
            var offset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                diceIndex * 0.2f, 
                Random.Range(-0.3f, 0.3f)
            );
            
            var spawnPosition = basePosition + offset;
            
            var dice = Instantiate(diceData.dicePrefab, spawnPosition, Quaternion.identity);
            dice.Init(diceData, diceIndex);

            var camTransform = mainCamera.transform;
            var throwDirection = camTransform.forward;
            throwDirection.y = 0; 
            throwDirection.Normalize();
            throwDirection.y = diceData.upwardAngle;

            dice.Throw(throwDirection, diceData.throwForce, diceData.torqueForce, targetValue);

            return dice;
        }

        private void OnDiceStopped(DiceStoppedEvent evt)
        {
            _diceStoppedCount++;
            _totalResult += evt.FaceValue;
        }

        private void CleanupDice()
        {
            foreach (var dice in _activeDice)
            {
                if (dice != null)
                    Destroy(dice.gameObject);
            }
            _activeDice.Clear();
        }
    }
}





