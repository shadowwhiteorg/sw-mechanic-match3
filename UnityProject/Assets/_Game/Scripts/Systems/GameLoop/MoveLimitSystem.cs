using _Game.Core.Events;
using _Game.Interfaces;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    public class MoveLimitSystem
    {
        public int MovesLeft { get; private set; }
        private readonly IEventBus _events;
        private bool _canCount = true;

        public MoveLimitSystem(int start, IEventBus events)
        {
            MovesLeft = start;
            _events = events;
            _events.Subscribe<TurnEndedEvent>(_ => UseMove());
            _events.Subscribe<BlockSelectedEvent>(e=>Activate(true));
            
        }

        private void UseMove()
        {
            if(!_canCount)return;
            Activate(false);
            MovesLeft = Mathf.Max(0, MovesLeft - 1);
            _events.Fire(new MoveUpdatedEvent(MovesLeft));
            if (MovesLeft == 0)
                _events.Fire(new GameOverEvent());
        }
        private void Activate(bool active)
        {
            _canCount = active;
        }
    }

}