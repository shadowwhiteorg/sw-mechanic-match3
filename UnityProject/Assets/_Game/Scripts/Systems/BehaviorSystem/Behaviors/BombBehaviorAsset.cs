using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using _Game.Utils;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Bomb Behavior")]
    public class BombBehaviorAsset : BlockBehaviorAsset
    {
        [Header("Clear Radius (Chebyshev)")]
        [SerializeField] private int radius = 1;

        private readonly HashSet<BlockModel> _exploded = new HashSet<BlockModel>();

        public override void OnPlaced(BlockModel block)
        {
            _exploded.Remove(block);
        }

        public override void OnActivated(BlockModel block)
        {
            base.OnActivated(block);
            if (!_exploded.Add(block))
                return;

            // Schedule the actual blast for next frame
            CoroutineRunner.Instance.StartCoroutine(DelayedExplode(block));
        }
        

        private IEnumerator DelayedExplode(BlockModel block)
        {
            Events.Fire(new BlockActivatedEvent());
            Explode(block);
            yield return null;
        }

        private void Explode(BlockModel block)
        {
            Debug.Log($"Bomb activated at {block.Row}, {block.Column}");

            int r0 = block.Row;
            int c0 = block.Column;

            for (int dr = -radius; dr <= radius; dr++)
            {
                for (int dc = -radius; dc <= radius; dc++)
                {
                    if (Mathf.Max(Mathf.Abs(dr), Mathf.Abs(dc)) > radius)
                        continue;

                    int rr = r0 + dr;
                    int cc = c0 + dc;

                    if (Grid.TryGet(rr, cc, out var neighbor))
                    {
                        // if (neighbor.Type != BlockType.None)
                        //     CoroutineRunner.Instance.StartCoroutine(WaitAndActivateBlock(rr, cc)); 
                        if (neighbor.Type != BlockType.None)
                            Events.Fire(new BlockSelectedEvent(rr,cc));
                        else
                            Events.Fire(new ClearBlockEvent(neighbor));
                        // Optional VFX or debug:
                        // neighbor.View.transform.localScale *= 0.5f;
                    }
                }
            }
            Events.Fire(new BlockDeactivatedEvent());
            Events.Fire(new ClearBlockEvent(block));
        }

        private IEnumerator WaitAndActivateBlock(int row, int column)
        {
            yield return new WaitForSeconds(0.5f);
            Events.Fire(new BlockSelectedEvent(row,column));
        }
    }
    
}
