using Core.Tokens;
using UnityEngine;

namespace Event
{
    public struct TokenMoveCompletedEvent : IEvent
    {
        public Token Token;
        public Vector2Int Coord;
    }

    public struct DiceStoppedEvent : IEvent
    {
        public int InstanceId;
        public int FaceValue;
    }

    public struct DiceRollCompletedEvent : IEvent
    {
        public int Dice1Value;
        public int Dice2Value;
        public int TotalValue;
    }
}