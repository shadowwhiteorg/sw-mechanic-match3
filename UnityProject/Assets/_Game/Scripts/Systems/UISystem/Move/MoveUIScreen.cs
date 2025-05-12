using _Game.Core.Events;
using _Game.Interfaces;

namespace _Game.Systems.UISystem.Move
{
    public class MoveUIScreen : BaseUIScreen<MoveUIModel, MoveUIView>
    {
        public override void Construct(MoveUIModel model, MoveUIView view, IEventBus eventBus)
        {
            base.Construct(model, view, eventBus);

            // Set initial moves when a level loads
            eventBus.Subscribe<LevelInitializedEvent>(e =>
            {
                model.SetMoves(e.LevelData.MoveLimit);
                Show();
            });

            // Update whenever the MoveLimitSystem fires a MoveUpdatedEvent
            eventBus.Subscribe<MoveUpdatedEvent>(e =>
            {
                model.SetMoves(e.MovesLeft);
            });  // :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}

            // Hide on win or loss
            eventBus.Subscribe<LevelCompleteEvent>(_ => Hide());
            eventBus.Subscribe<GameOverEvent>(_    => Hide());
        }
    }
}