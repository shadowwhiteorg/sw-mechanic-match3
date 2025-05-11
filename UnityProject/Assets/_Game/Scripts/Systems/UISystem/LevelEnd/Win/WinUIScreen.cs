using _Game.Core.Events;
using _Game.Interfaces;

namespace _Game.Systems.UISystem
{
    public class WinUIScreen : BaseUIScreen<WinUIModel, WinUIView>
    {
        public override void Construct(WinUIModel model, WinUIView view, IEventBus eventBus)
        {
            base.Construct(model, view, eventBus);

            // Show on level complete
            eventBus.Subscribe<LevelCompleteEvent>(_ =>
            {
                model.SetMessage("Level Complete!");
                Show();
            });

            // Buttons
            view.OnNextClicked  += () => eventBus.Fire(new NextLevelEvent());
            view.OnRetryClicked += () => eventBus.Fire(new RetryLevelEvent());
        }
    }
}