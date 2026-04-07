using System.Collections;
using System.Collections.Generic;
using Core.Pool;
using Event;
using Service;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Dice
{
    public class DiceController : MonoBehaviour, IDiceController
    {
        [Header("Configuration")]
        [SerializeField] private DiceData diceData;
        [SerializeField] private CinemachineCamera mainCamera;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnHeight = 0.5f;
        [SerializeField] private float spawnForwardOffset = 2f;
        [SerializeField] private float diceSpacing = 0.5f;

        private readonly List<DiceBehaviour> _activeDice = new();
        private IPoolService _poolService;
        
        private int _diceInstanceCounter;
        private int _stoppedDiceCount;
        private int _expectedDiceCount;
        private int _dice1Value;
        private int _dice2Value;

        private IPoolService PoolService => _poolService ??= ServiceLocator.Get<IPoolService>();

        private void OnEnable()
        {
            EventBus.Subscribe<DiceStoppedEvent>(OnDiceStopped);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DiceStoppedEvent>(OnDiceStopped);
        }

        public void RollDice(int dice1Value, int dice2Value)
        {
            ClearDice();

            _dice1Value = dice1Value;
            _dice2Value = dice2Value;
            _stoppedDiceCount = 0;
            _expectedDiceCount = 2;

            StartCoroutine(RollSequence(dice1Value, dice2Value));
        }

        private IEnumerator RollSequence(int dice1Value, int dice2Value)
        {
            SpawnAndThrowDice(dice1Value, -diceSpacing * 0.5f);
            yield return new WaitForSeconds(diceData.delayBetweenDice);
            SpawnAndThrowDice(dice2Value, diceSpacing * 0.5f);
        }

        private void SpawnAndThrowDice(int targetValue, float horizontalOffset)
        {
            var camTransform = mainCamera.transform;
            
            var camForwardFlat = camTransform.forward;
            camForwardFlat.y = 0f;
            camForwardFlat.Normalize();
            
            var camRightFlat = camTransform.right;
            camRightFlat.y = 0f;
            camRightFlat.Normalize();
            
            var spawnPos = camTransform.position + camForwardFlat * spawnForwardOffset;
            spawnPos.y = spawnHeight;
            spawnPos += camRightFlat * horizontalOffset;

            var dice = PoolService.Get(diceData.dicePrefab, spawnPos, Quaternion.identity);
            dice.Init(diceData, _diceInstanceCounter++);
            _activeDice.Add(dice);

            var throwDirection = (camForwardFlat + Vector3.up * 0.3f).normalized;
            dice.Throw(throwDirection, targetValue);
        }

        private void OnDiceStopped(DiceStoppedEvent evt)
        {
            _stoppedDiceCount++;

            if (_stoppedDiceCount >= _expectedDiceCount)
            {
                EventBus.Publish(new DiceRollCompletedEvent
                {
                    Dice1Value = _dice1Value,
                    Dice2Value = _dice2Value,
                    TotalValue = _dice1Value + _dice2Value
                });
            }
        }

        private void ClearDice()
        {
            foreach (var dice in _activeDice)
            {
                if (dice)
                {
                    dice.ResetDice();
                    PoolService.Return(diceData.dicePrefab, dice);
                }
            }
            _activeDice.Clear();
        }
    }
}




