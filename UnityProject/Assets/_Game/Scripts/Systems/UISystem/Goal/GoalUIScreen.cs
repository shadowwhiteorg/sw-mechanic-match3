using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;

namespace _Game.Systems.UISystem
{
    public class GoalUIScreen : BaseUIScreen<GoalUIModel, GoalUIView>
    {
        public override void Construct(GoalUIModel model, GoalUIView view, IEventBus eventBus)
        {
            base.Construct(model, view, eventBus);

            eventBus.Subscribe<LevelInitializedEvent>(e =>
            {
                model.SetGoals(e.LevelData.ColorGoals, e.LevelData.TypeGoals);
                Show();
            });

            eventBus.Subscribe<GoalUpdatedEvent>(e =>
            {
                if (e.Category == GoalUpdatedEvent.GoalCategory.Color)
                {
                    model.UpdateColorGoal((BlockColor)e.Id, e.Remaining);
                }
                else
                {
                    model.UpdateTypeGoal((BlockType)e.Id, e.Remaining);
                }
            });

            eventBus.Subscribe<LevelCompleteEvent>(_ =>
            {
                Hide();
            });
        }
    }
}