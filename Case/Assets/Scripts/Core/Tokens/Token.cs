using System.Collections;
using Core.Data;
using UnityEngine;

namespace Core.Tokens
{
    public class Token : MonoBehaviour
    {
        public TokenData tokenData;
        public Vector2Int currentCoord;
        public bool isMoving;

        public void Init(TokenData data, Vector2Int startCoord)
        {
            tokenData = data;
            currentCoord = startCoord;
            gameObject.name = tokenData.displayName;
        }
        
        public void SetPosition(Vector3 worldPosition) => transform.position = worldPosition + Vector3.up * tokenData.heightOffset;

        public void MoveTo(Vector3 targetPosition, Vector2Int targetCoord)
        {
            StartCoroutine(MoveCoroutine(targetPosition, targetCoord));
        }
        
        private IEnumerator MoveCoroutine(Vector3 targetPosition, Vector2Int targetCoord)
        {
            isMoving = true;
            var target = targetPosition + Vector3.up * tokenData.heightOffset;
            var start = transform.position;
            var elapsed = 0f;
            var duration = Vector3.Distance(start, target) / tokenData.moveSpeed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, target, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            transform.position = target;
            currentCoord = targetCoord;
            isMoving = false;

            EventBus.EventBus.Publish(new EventBus.TokenMoveCompletedEvent
            {
                Token = this,
                Coord = targetCoord
            });
        }
    }
}