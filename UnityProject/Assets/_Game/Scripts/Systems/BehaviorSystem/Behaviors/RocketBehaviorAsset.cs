using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Core.Events;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/RocketBehavior")]
    public class RocketBehaviorAsset : BlockBehaviorAsset
    {
        [Header("Clearing")] [Tooltip("Time between clearing each successive block")] [SerializeField]
        private float perBlockDelay = 0.05f;

        [Header("Appearance")] [Tooltip("Color to tint rocket blocks")] 
        [SerializeField] private Color rocketTint = Color.yellow;

        [Tooltip("Rotation (z) for vertical rockets")] [SerializeField]
        private float verticalRotationZ = 90f;

        // injected at Initialize(...)
        private IGridHandler _grid;
        private IEventBus _events;

        // per-instance state
        private readonly Dictionary<BlockModel, Vector2> _directions = new();

        public override void OnPlaced(BlockModel block)
        {
            // choose horizontal or vertical
            bool horiz = Random.value < 0.5f;
            var dir = horiz ? Vector2.right : Vector2.up;
            _directions[block] = dir;

            // tint & rotate the view so you can see it’s a rocket
            var sr = block.View.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = rocketTint;

            block.View.transform.rotation =
                Quaternion.Euler(0, 0, horiz ? 0f : verticalRotationZ);
        }

        public override void OnMatched(BlockModel block)
        {
            // start blasting outwards in both +dir and –dir
            CoroutineRunner.Instance.StartCoroutine(BlastSequence(block));
        }

        private IEnumerator BlastSequence(BlockModel block)
        {
            if (!_directions.TryGetValue(block, out var dir))
                yield break;

            int max = dir.x != 0 ? _grid.Columns : _grid.Rows;

            // for each step away from the center
            for (int i = 1; i < max; i++)
            {
                yield return new WaitForSeconds(perBlockDelay);

                int r = block.Row + (int)(dir.y * i);
                int c = block.Column + (int)(dir.x * i);

                // if there’s a block, ask to clear it
                if (_grid.TryGet(r, c, out var target))
                    _events.Fire(new ClearBlockEvent(target));
                else
                    break; // off grid → stop
            }
        }

        public override void OnCleared(BlockModel block)
        {
            // rocket itself will be cleared by ClearSystem when you fire the first ClearBlockEvent
            _directions.Remove(block);
        }
        
    }
}

