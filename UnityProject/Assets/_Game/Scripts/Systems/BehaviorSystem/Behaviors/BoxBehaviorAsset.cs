// FILE: BoxBehaviorAsset.cs (fixed)
using System.Collections;
using System.Collections.Generic;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Box")]
    public class BoxBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private int maxHealth = 1;

        // Per-block runtime state lives here (asset is shared; state must not be).
        private readonly Dictionary<BlockModel, BoxState> _state = new();

        private struct BoxState
        {
            public int Health;
            public bool Destroyed;
        }

        public override void OnPlaced(BlockModel block)
        {
            base.OnPlaced(block);
            _state[block] = new BoxState
            {
                Health = Mathf.Max(1, maxHealth),
                Destroyed = false
            };

            // Optional: debug
            Debug.Log($"[Box] Placed at ({block.Row},{block.Column}) with {maxHealth} HP");
        }

        public override void OnActivated(BlockModel block)
        {
            base.OnActivated(block);
        }

        public override void OnMatched(BlockModel block)
        {
            // Boxes are not cleared by color-matches; only by damage.
        }

        public override void OnFell(BlockModel block)
        {
            // Boxes typically don't fall; no-op. Keep if your game allows.
        }

        public override void OnCleared(BlockModel block)
        {
            // Clean up state when the block is finally removed.
            if (_state.Remove(block))
            {
                // Optional: debug
                Debug.Log($"[Box] Cleared at ({block.Row},{block.Column}) – state removed");
            }
        }

        // IMPORTANT: CanClear must be per-block, not from asset-wide flags.
        public override bool CanClear(BlockModel block)
        {
            if (!_state.TryGetValue(block, out var s))
                return false; // if unknown, treat as not clearable
            return s.Destroyed;
        }

        /// <summary>
        /// Apply damage to this *specific* block.
        /// Always pass the block you're damaging (blast neighbor, bomb neighbor, rocket sweep neighbor).
        /// </summary>
        public void TakeDamage(int damage, DamageSource source, BlockModel block)
        {
            if (block == null) return;

            if (!_state.TryGetValue(block, out var s))
            {
                // Late-damage edge case: ensure we have state.
                s = new BoxState { Health = Mathf.Max(1, maxHealth), Destroyed = false };
            }

            if (s.Destroyed)
            {
                // Already dead; ignore further damage.
                return;
            }

            s.Health -= Mathf.Max(1, damage);

            Debug.Log($"[Box] Damage {damage} from {source} at ({block.Row},{block.Column}) => HP {s.Health}");

            if (s.Health <= 0)
            {
                s.Destroyed = true;
                _state[block] = s;

                // Notify destruction first (for VFX/SFX), then schedule clear next frame
                Events.Fire(new BlockDestroyedEvent(block, source));

                // Delay ClearBlockEvent by one frame so ClearSystem.CanClear(block) sees Destroyed=true
                CoroutineRunner.Instance.StartCoroutine(DelayedClear(block));
            }
            else
            {
                _state[block] = s;
            }
        }

        private IEnumerator DelayedClear(BlockModel block)
        {
            yield return null; // one frame
            // ClearSystem will call CanClear(block) and succeed now that Destroyed==true
            Events.Fire(new ClearBlockEvent(block));
        }
    }
}