using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Balloon")]
    public class BalloonBehaviorAsset : BlockBehaviorAsset
    {

        public override void Initialize(
            IGridHandler grid,
            IBlockFactory factory,
            GridWorldHelper helper,
            IEventBus events)
        {
            base.Initialize(grid, factory, helper, events);
            events.Subscribe<MatchFoundEvent>(OnAnyMatch);
        }
        
        public override void OnActivated(BlockModel block)
        {
            base.OnActivated(block);
            Events.Fire(new ClearBlockEvent(block));
        }

        public override void OnCleared(BlockModel block)
        {
            base.OnCleared(block);
            Events.Fire(new ClearBlockEvent(block));
        }

        private void OnAnyMatch(MatchFoundEvent e)
        {
            foreach (var matched in e.Blocks)
            {
                // for each orthogonal neighbor of the matched block
                foreach (var (dr, dc) in new[] { (-1,0),(1,0),(0,-1),(0,1) })
                {
                    int nr = matched.Row + dr;
                    int nc = matched.Column + dc;

                    if (Grid.TryGet(nr, nc, out var neighbor)
                        && neighbor.Type == BlockType.Balloon)
                    {
                        // trigger its clear path
                        Events.Fire(new ClearBlockEvent(neighbor));
                    }
                }
            }
        }

    }
}
