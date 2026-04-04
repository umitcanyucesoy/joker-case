using System;
using System.Collections;
using System.Collections.Generic;
using Core.Data;
using Core.Grid;
using EventBus;
using Service;
using UnityEngine;

namespace Core.Tokens
{
    public class TokenController : MonoBehaviour, ITokenController
    {
        [Serializable]
        public class TokenSpawnConfig
        {
            public TokenData tokenData;
            public Vector2Int startCoord;
        }

        [Header("Spawn Configuration")]
        [SerializeField] private List<TokenSpawnConfig> tokensToSpawn = new();

        private readonly Dictionary<Vector2Int, Token> _tokenPositions = new();
        private readonly List<Token> _allTokens = new();
        private IGridService _gridService;
        private Token _activeToken;
        private bool _isMoving;

        private void OnEnable()
        {
            EventBus.EventBus.Subscribe<TokenMoveCompletedEvent>(OnTokenMoveCompleted);
        }

        private void OnDisable()
        {
            EventBus.EventBus.Unsubscribe<TokenMoveCompletedEvent>(OnTokenMoveCompleted);
        }

        public void Initialize()
        {
            _gridService = ServiceLocator.Get<IGridService>();

            foreach (var config in tokensToSpawn)
            {
                SpawnToken(config.tokenData, config.startCoord);
            }

            if (_allTokens.Count > 0)
                _activeToken = _allTokens[0];
        }

        public void SpawnToken(TokenData tokenData, Vector2Int startCoord)
        {
            if (!_gridService.TryGetTileWorldPosition(startCoord, out var worldPos))
            {
                Debug.LogError($"[TokenController] Tile not found at {startCoord}");
                return;
            }

            var token = Instantiate(tokenData.prefab);
            token.Init(tokenData, startCoord);
            token.SetPosition(worldPos);
            _tokenPositions[startCoord] = token;
            _allTokens.Add(token);

            Debug.Log($"[TokenController] Spawned {tokenData.displayName} at {startCoord}");
        }

        public void MoveActiveTokenBySteps(int steps)
        {
            if (_activeToken == null || _isMoving)
            {
                Debug.LogWarning("[TokenController] No active token or already moving");
                return;
            }

            StartCoroutine(MoveTokenStepByStep(_activeToken, steps));
        }

        private IEnumerator MoveTokenStepByStep(Token token, int steps)
        {
            _isMoving = true;
            var currentCoord = token.currentCoord;

            for (var i = 0; i < steps; i++)
            {
                var nextCoord = GetNextCoord(currentCoord);
                
                if (!_gridService.TryGetTileWorldPosition(nextCoord, out var worldPos))
                {
                    Debug.Log($"[TokenController] Reached grid boundary at {currentCoord}");
                    break;
                }

                _tokenPositions.Remove(currentCoord);
                token.MoveTo(worldPos, nextCoord);

                // Wait for move to complete
                yield return new WaitUntil(() => !token.isMoving);
                yield return new WaitForSeconds(0.1f); // Small delay between steps

                currentCoord = nextCoord;
            }

            _isMoving = false;
            Debug.Log($"[TokenController] {token.tokenData.displayName} finished at {currentCoord}");
        }

        private Vector2Int GetNextCoord(Vector2Int current)
        {
            var x = current.x;
            var y = current.y;

            // Even row (0, 2, 4...): move right (x increases)
            // Odd row (1, 3, 5...): move left (x decreases)
            var isEvenRow = y % 2 == 0;
            var maxX = _gridService.Columns - 1;

            if (isEvenRow)
            {
                // Moving right
                if (x < maxX)
                    return new Vector2Int(x + 1, y);
                else
                    return new Vector2Int(x, y + 1); // Go up
            }
            else
            {
                // Moving left
                if (x > 0)
                    return new Vector2Int(x - 1, y);
                else
                    return new Vector2Int(x, y + 1); // Go up
            }
        }

        public void MoveToken(Token token, Vector2Int targetCoord)
        {
            if (!_gridService.TryGetTileWorldPosition(targetCoord, out var worldPos))
            {
                Debug.LogError($"[TokenController] Target tile not found at {targetCoord}");
                return;
            }

            _tokenPositions.Remove(token.currentCoord);
            token.MoveTo(worldPos, targetCoord);
        }

        private void OnTokenMoveCompleted(TokenMoveCompletedEvent evt)
        {
            _tokenPositions[evt.Coord] = evt.Token;
        }

        public bool TryGetTokenAt(Vector2Int coord, out Token token)
        {
            return _tokenPositions.TryGetValue(coord, out token);
        }
    }
}
