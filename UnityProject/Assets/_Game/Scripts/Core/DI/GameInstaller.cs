using _Game.Interfaces;
using _Game.Scripts.Interfaces;
using _Game.Systems;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Core.DI
{
    public class GameInstaller : MonoBehaviour
    {
        [Header("Grid & Block Settings")]
        [SerializeField] private GridConfig gridConfig;
        [SerializeField] private BlockTypeConfig blockTypeConfig;
        [SerializeField] private BlockView blockPrefab;
        [SerializeField] private Transform blockParent;
        
        
        public void Initialize(IDIContainer container, IEventBus eventBus)
        {
            var grid = new GridHandler(gridConfig.Rows, gridConfig.Columns);
            var factory     = new BlockFactory(blockTypeConfig, blockPrefab, blockParent);
            var initializer = new GridInitializer(grid, factory, gridConfig);
            var helper = new GridWorldHelper(gridConfig, blockParent.position);
            container.BindSingleton<GridWorldHelper>(helper);
            container.BindSingleton<IGridHandler>(grid);
            
            // 4) Clear & Fall
            var clearSys = new ClearSystem(grid, factory, eventBus);
            container.BindSingleton(clearSys);

            var fallSys = new FallSystem(grid, helper, factory, eventBus,gridConfig);
            container.BindSingleton(fallSys);
            

            initializer.InitializeGrid();
        }
    }
}