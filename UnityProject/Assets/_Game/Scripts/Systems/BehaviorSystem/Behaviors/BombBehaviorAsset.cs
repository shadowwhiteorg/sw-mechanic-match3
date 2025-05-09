using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
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
            Explode(block);
        }

        public override void OnCleared(BlockModel block)
        {
            if (!_exploded.Add(block))
                return;

            // Schedule the actual blast for next frame
            CoroutineRunner.Instance.StartCoroutine(DelayedExplode(block));
        }

        private IEnumerator DelayedExplode(BlockModel block)
        {

            Explode(block);
            yield return null;
        }

        private void Explode(BlockModel block)
        {
            Debug.Log($"Bomb activated at {block.Row}, {block.Column}");
            int r0 = block.Row;
            int c0 = block.Column;
            
            // Chebyshev radius: includes diagonals
            for (int dr = -radius; dr <= radius; dr++)
            {
                for (int dc = -radius; dc <= radius; dc++)
                {
                    if (Mathf.Max(Mathf.Abs(dr), Mathf.Abs(dc)) > radius)
                        continue;
            
                    if (Grid.TryGet(r0 + dr, c0 + dc, out var neighbor))
                        Events.Fire(new ClearBlockEvent(neighbor));
                        // neighbor.View.transform.localScale*=0.5f;
                }
            }
            
        }
    }
}
