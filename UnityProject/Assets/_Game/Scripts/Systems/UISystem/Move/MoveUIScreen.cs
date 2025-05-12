using _Game.Core.Events;
using _Game.Interfaces;

namespace _Game.Systems.UISystem.Move
{
    public class MoveUIScreen : BaseUIScreen<MoveUIModel, MoveUIView>
    {
        public override void Construct(MoveUIModel model, MoveUIView view, IEventBus eventBus)
        {
            base.Construct(model, view, eventBus);

            eventBus.Subscribe<LevelInitializedEvent>(e =>
            {
                model.SetMoves(e.LevelData.MoveLimit);
                Show();
            });

            eventBus.Subscribe<MoveUpdatedEvent>(e =>
            {
                model.SetMoves(e.MovesLeft);
            });  

            eventBus.Subscribe<LevelCompleteEvent>(_ => Hide());
            eventBus.Subscribe<GameOverEvent>(_    => Hide());
        }
    }
}