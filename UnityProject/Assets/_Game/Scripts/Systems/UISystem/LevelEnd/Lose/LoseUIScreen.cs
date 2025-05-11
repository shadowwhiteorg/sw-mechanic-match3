using _Game.Core.Events;
using _Game.Interfaces;

namespace _Game.Systems.UISystem
{
    public class LoseUIScreen : BaseUIScreen<LoseUIModel, LoseUIView>
    {
        public override void Construct(LoseUIModel model, LoseUIView view, IEventBus eventBus)
        {
            base.Construct(model, view, eventBus);

            // Show on game over
            eventBus.Subscribe<GameOverEvent>(_ =>
            {
                model.SetMessage("Game Over");
                Show();
            });

            // Button
            view.OnRetryClicked += () => eventBus.Fire(new RetryLevelEvent());
        }
    }
}