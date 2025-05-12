using System.Linq;
using UnityEngine;
using _Game.Enums;
using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Systems.BehaviorSystem;
using _Game.Systems.BlockSystem;
using _Game.Utils;

[CreateAssetMenu(menuName="Blast/Behaviors/Duck")]
public class DuckBehaviorAsset : BlockBehaviorAsset
{
    private IGridHandler _grid;
    private IEventBus _events;

    public override void Initialize(
        IGridHandler grid,
        IBlockFactory factory,
        GridWorldHelper helper,
        IEventBus eventBus)
    {
        _grid   = grid;
        _events = eventBus;

        // whenever a column finishes falling, check for delivery
        _events.Subscribe<TurnEndedEvent>(_=> OnTurnEnd());
    }

    public override void OnPlaced(BlockModel block)
    {
        // nothing special here beyond default
    }

    public override bool CanClear(BlockModel block)
    {
        // allow removal only if there are no blocks below
        int c = block.Column;
        for (int r = block.Row + 1; r < _grid.Rows; r++)
            if (_grid.GetBlock(r, c) != null)
                return false;
        return true;
    }

    public override void OnCleared(BlockModel block)
    {
        // Debug.Log($"Duck cleared at {block.Row}, {block.Column}");
    }

    public override void OnTurnEnd()
    {
        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Columns; c++)
            {
                if (_grid.GetBlock(r, c) is BlockModel blk 
                    && blk.Type == BlockType.Duck 
                    && CanClear(blk))
                {
                    _events.Fire(new ClearBlockEvent(blk));
                }
            }
        }
    }
}