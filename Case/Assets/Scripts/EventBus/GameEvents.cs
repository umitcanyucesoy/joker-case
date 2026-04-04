using Core.Tokens;
using UnityEngine;

namespace EventBus
{
    public struct TokenMoveCompletedEvent : IEvent
    {
        public Token Token;
        public Vector2Int Coord;
    }
}