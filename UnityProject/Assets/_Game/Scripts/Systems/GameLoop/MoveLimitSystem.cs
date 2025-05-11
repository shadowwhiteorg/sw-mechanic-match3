using _Game.Core.Events;
using _Game.Interfaces;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    public class MoveLimitSystem
    {
        public int MovesLeft { get; private set; }
        private readonly IEventBus _events;

        public MoveLimitSystem(int start, IEventBus events)
        {
            MovesLeft = start;
            _events = events;
            _events.Subscribe<TurnEndedEvent>(_ => UseMove());
        }

        private void UseMove()
        {
            
            MovesLeft = Mathf.Max(0, MovesLeft - 1);
            Debug.Log($"Moves left: {MovesLeft}");
            _events.Fire(new MoveUpdatedEvent(MovesLeft));
            if (MovesLeft == 0)
                _events.Fire(new GameOverEvent());
        }
    }

}