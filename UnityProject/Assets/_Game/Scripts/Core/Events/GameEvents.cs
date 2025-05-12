using _Game.Interfaces;
using _Game.Systems.GameLoop;
using UnityEngine;

namespace _Game.Core.Events
{
    public struct GameStartedEvent : IGameEvent
    {
        public string Message { get; }
        public GameStartedEvent(string message) => Message = message;
    }

    public struct LevelInitializedEvent : IGameEvent
    {
        public int Level { get; }
        public LevelData LevelData { get; }

        public LevelInitializedEvent(int level, LevelData levelData)
        {
            Level = level;
            LevelData = levelData;
        }
    }
    
    public struct TurnEndedEvent : IGameEvent
    {
    }

    public struct GoalUpdatedEvent : IGameEvent
    {
        public enum GoalCategory
        {
            Color,
            Type
        }

        public readonly GoalCategory Category;
        public readonly int Id; // enum value of color or type
        public readonly int Remaining;

        public GoalUpdatedEvent(GoalCategory cat, int id, int rem)
        {
            Category = cat;
            Id = id;
            Remaining = rem;
        }
    }
    public struct GoalCollectedEvent : IGameEvent{}

    public struct LevelCompleteEvent : IGameEvent
    {
        public int Level { get; }

        public LevelCompleteEvent(int level)
        {
            Level = level;
            Debug.Log($"Level {level} complete!");
        }
    }

    public struct GameOverEvent : IGameEvent
    {
    }

    public struct MoveUpdatedEvent : IGameEvent
    {
        public int MovesLeft { get; }
        public MoveUpdatedEvent(int movesLeft) => MovesLeft = movesLeft;
    }



    public struct NextLevelEvent : IGameEvent
    {
        public int Level { get; }
        public NextLevelEvent(int level) => Level = level;
    }

    public struct RetryLevelEvent : IGameEvent
    {
    }
}