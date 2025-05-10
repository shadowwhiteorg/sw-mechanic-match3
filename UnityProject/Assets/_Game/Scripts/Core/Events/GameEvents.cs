using _Game.Interfaces;

namespace _Game.Core.Events
{
    public struct GameStartedEvent : IGameEvent
    {
        public string Message { get; }
        public GameStartedEvent(string message) => Message = message;
    }
    public struct TurnStartedEvent : IGameEvent
    {
    }
    public struct TurnEndedEvent : IGameEvent
    {
    }
    
    public struct GoalUpdatedEvent : IGameEvent
    {
        public enum GoalCategory { Color, Type }
        public readonly GoalCategory Category;
        public readonly int          Id;        // enum value of color or type
        public readonly int          Remaining;

        public GoalUpdatedEvent(GoalCategory cat, int id, int rem)
        {
            Category  = cat;
            Id        = id;
            Remaining = rem;
        }
    }

    public struct LevelCompleteEvent : IGameEvent { }
    
    public struct GameOverEvent: IGameEvent { }
    
    public struct MoveUpdatedEvent : IGameEvent
    {
        public int MovesLeft { get; }
        public MoveUpdatedEvent(int movesLeft) => MovesLeft = movesLeft;
    }
}