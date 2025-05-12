using UnityEngine;
using _Game.Interfaces;
using _Game.Systems.GridSystem;
using _Game.Systems.BlockSystem;
using _Game.Systems.MatchSystem;
using _Game.Systems.CoreSystems;
using _Game.Systems.BehaviorSystem;
using _Game.systems.BlockSystem;
using _Game.Systems.GameLoop;
using _Game.Utils;

namespace _Game.Core.DI
{
    public class GameInstaller : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private GridConfig gridConfig;
        [SerializeField] private BlockTypeConfig blockTypeConfig;
        [SerializeField] private BehaviorConfig behaviorConfig;
        [SerializeField] private SpecialBlockSpawnConfig spawnConfig;

        [Header("Visuals")] [SerializeField] private BlockView blockPrefab;
        [SerializeField] private Transform blockParent;
        

        public void Initialize(IDIContainer container, IEventBus eventBus, LevelManager levelManager)
        {
            var level = container.Resolve<LevelData>();

            // Override grid config with LevelData
            gridConfig.Rows = level.Rows;
            gridConfig.Columns = level.Columns;

            // Grid + helper
            var grid = new GridHandler(gridConfig.Rows, gridConfig.Columns);
            var helper = new GridWorldHelper(gridConfig, blockParent.position);
            container.BindSingleton<IGridHandler>(grid);
            container.BindSingleton(helper);

            // Behavior registry
            var registry = new BehaviorRegistry(behaviorConfig.behaviors);
            container.BindSingleton(registry);
            
            // Factory
            var factory = new BlockFactory(
                blockTypeConfig,
                blockPrefab,
                blockParent,
                helper,
                grid,
                eventBus,
                registry);
            container.BindSingleton<IBlockFactory>(factory);

            // Core systems
            container.BindSingleton(new ClearSystem(grid, factory, eventBus, spawnConfig));
            container.BindSingleton(new FallSystem(grid, helper, factory, eventBus));
            container.BindSingleton<IUpdatableSystem>(
                new MatchSystem(grid, eventBus, gridConfig.MatchThreshold));

            // Gameplay systems
            container.BindSingleton(new GoalSystem(level, eventBus,levelManager));
            container.BindSingleton(new MoveLimitSystem(level.MoveLimit, eventBus));

            // Input
            var input = new InputSystem(Camera.main, helper, eventBus);
            container.BindSingleton<IUpdatableSystem>(input);
            container.Resolve<ISystemRunner>().Register(input);
            
            var shuffle = new ShuffleSystem(
                grid,
                eventBus,
                gridConfig,
                factory,
                container.Resolve<GridWorldHelper>()
            );
            container.BindSingleton(shuffle);
            
            // Prepare grid
            var init = new GridInitializer(grid, factory, gridConfig);
            init.InitializeGrid();
        }
    }
}