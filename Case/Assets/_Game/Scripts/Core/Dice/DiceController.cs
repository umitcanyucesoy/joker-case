using System.Collections;
using System.Collections.Generic;
using Core.Enums;
using Core.Particles;
using Core.Pool;
using Core.Sound;
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

        [Header("Smoke Effect")]
        [SerializeField] private float smokeSideOffset = 0.3f;

        private readonly List<DiceBehaviour> _activeDice = new();
        private IPoolService _poolService;
        private IParticleService _particleService;
        private ISoundService _soundService;
        
        private int _diceInstanceCounter;
        private int _stoppedDiceCount;
        private int _expectedDiceCount;
        private int[] _diceValues;
        private bool _collisionHandled;

        private IPoolService PoolService => _poolService ??= ServiceLocator.Get<IPoolService>();
        private IParticleService ParticleService => _particleService ??= ServiceLocator.Get<IParticleService>();
        private ISoundService SoundService => _soundService ??= ServiceLocator.Get<ISoundService>();

        private void OnEnable()
        {
            EventBus.Subscribe<DiceStoppedEvent>(OnDiceStopped);
            EventBus.Subscribe<DiceCollisionEvent>(OnDiceCollision);
            EventBus.Subscribe<DiceGroundHitEvent>(OnDiceGroundHit);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DiceStoppedEvent>(OnDiceStopped);
            EventBus.Unsubscribe<DiceCollisionEvent>(OnDiceCollision);
            EventBus.Unsubscribe<DiceGroundHitEvent>(OnDiceGroundHit);
        }

        public void RollDice(int dice1Value, int dice2Value) => RollDice(new[] { dice1Value, dice2Value });
        
        public void RollDice(int[] diceValues)
        {
            ClearDice();

            _diceValues = diceValues;
            _stoppedDiceCount = 0;
            _expectedDiceCount = diceValues.Length;
            _collisionHandled = false;

            SoundService.Play(SoundType.DiceRoll);
            StartCoroutine(RollSequence(diceValues));
        }

        private IEnumerator RollSequence(int[] diceValues)
        {
            var count = diceValues.Length;
            var totalWidth = (count - 1) * diceSpacing;
            var startOffset = -totalWidth * 0.5f;

            for (var i = 0; i < count; i++)
            {
                var offset = startOffset + i * diceSpacing;
                SpawnAndThrowDice(diceValues[i], offset);

                if (i < count - 1)
                    yield return new WaitForSeconds(diceData.delayBetweenDice);
            }
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
                var total = 0;
                foreach (var v in _diceValues) total += v;

                EventBus.Publish(new DiceRollCompletedEvent
                {
                    DiceValues = _diceValues,
                    TotalValue = total
                });
            }
        }

        private void OnDiceCollision(DiceCollisionEvent evt)
        {
            if (_collisionHandled) return;
            _collisionHandled = true;

            ParticleService.Play(ParticleType.LandHit, evt.ContactPoint);
        }

        private void OnDiceGroundHit(DiceGroundHitEvent evt)
        {
            var right = mainCamera.transform.right;
            right.y = 0f;
            right.Normalize();

            var pos = evt.ContactPoint + right * smokeSideOffset;
            ParticleService.Play(ParticleType.Smoke, pos);
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




