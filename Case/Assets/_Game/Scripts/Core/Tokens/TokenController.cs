using System;
using System.Collections;
using System.Collections.Generic;
using Core.Camera;
using Core.Data;
using Core.Enums;
using Core.Grid;
using Core.Sound;
using Event;
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
        private ICameraController _cameraController;
        private ISoundService _soundService;
        private Token _activeToken;
        private bool _isMoving;

        private void OnEnable()
        {
           EventBus.Subscribe<TokenMoveCompletedEvent>(OnTokenMoveCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<TokenMoveCompletedEvent>(OnTokenMoveCompleted);
        }

        public void Initialize(ICameraController cameraController)
        {
            _gridService = ServiceLocator.Get<IGridService>();
            _soundService = ServiceLocator.Get<ISoundService>();
            _cameraController = cameraController;
            
            foreach (var config in tokensToSpawn) SpawnToken(config.tokenData, config.startCoord);
            if (_allTokens.Count > 0) _activeToken = _allTokens[0];
            
            _cameraController.SetFollowTarget(_activeToken.transform);
        }

        public void SpawnToken(TokenData tokenData, Vector2Int startCoord)
        {
            if (!_gridService.TryGetTileWorldPosition(startCoord, out var worldPos)) return;

            var token = Instantiate(tokenData.prefab);
            token.Init(tokenData, startCoord);
            token.SetPosition(worldPos);
            _tokenPositions[startCoord] = token;
            _allTokens.Add(token);
        }

        public void MoveActiveTokenBySteps(int steps)
        {
            if (!_activeToken || _isMoving) return;
            StartCoroutine(MoveTokenStepByStep(_activeToken, steps));
        }

        private IEnumerator MoveTokenStepByStep(Token token, int steps)
        {
            _isMoving = true;
            var currentCoord = token.currentCoord;

            for (var i = 0; i < steps; i++)
            {
                var nextCoord = GetNextCoord(currentCoord);
                
                if (IsLastTile(currentCoord))
                {
                    var startCoord = new Vector2Int(0, 0);
                    if (_gridService.TryGetTileWorldPosition(startCoord, out var startPos))
                    {
                        _tokenPositions.Remove(currentCoord);
                        _soundService.Play(SoundType.TokenMove);
                        token.LapBounceTo(startPos, startCoord);
                        
                        yield return new WaitUntil(() => !token.isMoving);
                        yield return new WaitForSeconds(token.tokenData.stepDelay);
                        
                        currentCoord = startCoord;
                        continue;
                    }
                }
                
                if (!_gridService.TryGetTileWorldPosition(nextCoord, out var worldPos)) break;

                _tokenPositions.Remove(currentCoord);
                _soundService.Play(SoundType.TokenMove);
                token.MoveTo(worldPos, nextCoord);

                yield return new WaitUntil(() => !token.isMoving);
                yield return new WaitForSeconds(token.tokenData.stepDelay); 

                currentCoord = nextCoord;
            }

            _isMoving = false;
            
            if (_gridService.TryGetTile(currentCoord, out var tile))
                Debug.Log($"[TokenController] Landed on Tile #{tile.TileNumber} ({currentCoord})");
            
            CollectItemOnTile(currentCoord);
            EventBus.Publish(new TokenSequenceCompletedEvent());
        }

        private void CollectItemOnTile(Vector2Int coord)
        {
            if (!_gridService.TryGetTile(coord, out var tile)) return;
            if (!tile.currentType || tile.currentType.tileType == TileType.None || tile.currentCount <= 0) return;

            _soundService.Play(SoundType.ItemCollect);

            EventBus.Publish(new ItemCollectedEvent
            {
                ItemType = tile.currentType.tileType,
                TypeData = tile.currentType,
                Count = tile.currentCount
            });
        }

        private bool IsLastTile(Vector2Int coord)
        {
            var maxRow = _gridService.Rows - 1;
            var isEvenRow = maxRow % 2 == 0;
            
            if (coord.y != maxRow) return false;
            
            return isEvenRow ? coord.x == _gridService.Columns - 1 : coord.x == 0;
        }

        private Vector2Int GetNextCoord(Vector2Int current)
        {
            var x = current.x;
            var y = current.y;

            var isEvenRow = y % 2 == 0;
            var maxX = _gridService.Columns - 1;

            if (isEvenRow)
            {
                if (x < maxX)
                    return new Vector2Int(x + 1, y);
                else
                    return new Vector2Int(x, y + 1); 
            }
            else
            {
                if (x > 0)
                    return new Vector2Int(x - 1, y);
                else
                    return new Vector2Int(x, y + 1); 
            }
        }

        public void MoveToken(Token token, Vector2Int targetCoord)
        {
            if (!_gridService.TryGetTileWorldPosition(targetCoord, out var worldPos)) return;

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
