using Core.Data;
using Core.Enums;
using Core.Tokens;
using UnityEngine;

namespace Event
{
    public struct TokenMoveCompletedEvent : IEvent
    {
        public Token Token;
        public Vector2Int Coord;
        public bool IsLapComplete;
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
    
    public struct ItemCollectedEvent : IEvent
    {
        public TileType ItemType;
        public TileTypeData TypeData;
        public int Count;
    }

    public struct CollectAnimationCompletedEvent : IEvent
    {
        public TileType ItemType;
        public int Count;
    }
}