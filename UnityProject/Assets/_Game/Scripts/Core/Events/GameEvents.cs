using _Game.Interfaces;

namespace _Game.Core.Events
{
    public class GameStartedEvent : IGameEvent
    {
        public string Message { get; }
        public GameStartedEvent(string message) => Message = message;
    }
}