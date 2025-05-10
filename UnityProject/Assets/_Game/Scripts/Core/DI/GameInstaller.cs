// Core/DI/GameInstaller.cs
using UnityEngine;
using _Game.Interfaces;
using _Game.Systems.GridSystem;
using _Game.Systems.BlockSystem;
using _Game.Systems.MatchSystem;
using _Game.Systems.CoreSystems;
using _Game.Core.Events;
using _Game.Systems.BehaviorSystem;
using _Game.systems.BlockSystem;
using _Game.Utils;

namespace _Game.Core.DI
{
    [RequireComponent(typeof(BehaviorConfig))]
    public class GameInstaller : MonoBehaviour
    {
        [Header("Configs & Prefabs")]
        [SerializeField] private GridConfig gridConfig;
        [SerializeField] private BlockTypeConfig blockTypeConfig;
        [SerializeField] private BehaviorConfig behaviorConfig;
        [SerializeField] private SpecialBlockSpawnConfig specialBlockSpawnConfig;
        [SerializeField] private BlockView blockPrefab;
        [SerializeField] private Transform blockParent;

        public void Initialize(IDIContainer container, IEventBus eventBus)
        {
            // Core grid setup
            var grid = new GridHandler(gridConfig.Rows, gridConfig.Columns);
            container.BindSingleton<IGridHandler>(grid);

            var helper = new GridWorldHelper(gridConfig, blockParent.position);
            container.BindSingleton(helper);

            var registry = new BehaviorRegistry(behaviorConfig.behaviors);
            container.BindSingleton(registry);

            var factory = new BlockFactory(blockTypeConfig, blockPrefab, blockParent, helper, grid, eventBus, registry);
            container.BindSingleton<IBlockFactory>(factory);

            // Core systems
            var clearSystem = new ClearSystem(grid, factory, eventBus, specialBlockSpawnConfig);
            container.BindSingleton(clearSystem);

            var matchSystem = new MatchSystem(grid, eventBus, gridConfig.MatchThreshold);
            container.BindSingleton<IUpdatableSystem>(matchSystem);


            var fallSystem = new FallSystem(grid, helper, factory, eventBus);
            container.BindSingleton(fallSystem);

            // var matchSystem = new MatchSystem(grid, eventBus, gridConfig.MatchThreshold);
            // container.BindSingleton<IUpdatableSystem>(matchSystem);

            // Initial grid spawn
            var initializer = new GridInitializer(grid, factory, gridConfig);
            initializer.InitializeGrid();
        }
    }
}