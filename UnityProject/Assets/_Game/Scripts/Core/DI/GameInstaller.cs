using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using UnityEngine;

namespace _Game.Core.DI
{
    public class GameInstaller : MonoBehaviour
    {
        [Header("Grid & Block Settings")]
        [SerializeField] private GridConfig      gridConfig;
        [SerializeField] private BlockTypeConfig blockTypeConfig;
        [SerializeField] private GameObject      blockPrefab;
        [SerializeField] private Transform       blockParent;

        public void Initialize()
        {
            var grid        = new GridSystem(gridConfig.Rows, gridConfig.Columns);
            var factory     = new BlockFactory(blockTypeConfig, blockPrefab, blockParent);
            var initializer = new GridInitializer(grid, factory, gridConfig);

            initializer.InitializeGrid();
        }
    }
}