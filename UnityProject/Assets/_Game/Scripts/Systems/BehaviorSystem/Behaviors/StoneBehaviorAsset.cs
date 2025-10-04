// FILE: StoneBehaviorAsset.cs
using System.Collections;
using System.Collections.Generic;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Stone")]
    public class StoneBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private int maxHealth = 1;

        // Per-block runtime state lives here (asset is shared; state must not be).
        private readonly Dictionary<BlockModel, StoneState> _state = new();

        private struct StoneState
        {
            public int Health;
            public bool Destroyed;
        }

        public override void OnPlaced(BlockModel block)
        {
            base.OnPlaced(block);
            _state[block] = new StoneState
            {
                Health = Mathf.Max(1, maxHealth),
                Destroyed = false
            };

            // Optional: debug
            Debug.Log($"[Stone] Placed at ({block.Row},{block.Column}) with {maxHealth} HP");
        }

        public override void OnActivated(BlockModel block)
        {
            base.OnActivated(block);
        }

        public override void OnMatched(BlockModel block)
        {
            // Stones are not cleared by color-matches; only by damage.
        }

        public override void OnFell(BlockModel block)
        {
            // Stones don't fall; no-op. Keep if your game allows.
        }

        public override void OnCleared(BlockModel block)
        {
            // Clean up state when the block is finally removed.
            if (_state.Remove(block))
            {
                // Optional: debug
                Debug.Log($"[Stone] Cleared at ({block.Row},{block.Column}) – state removed");
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
        /// Stones can only be damaged by rockets, not by bombs or blasts.
        /// </summary>
        public void TakeDamage(int damage, DamageSource source, BlockModel block)
        {
            if (block == null) return;

            // Stones can only be damaged by rockets
            if (source != DamageSource.Rocket)
            {
                Debug.Log($"[Stone] Blocked damage from {source} at ({block.Row},{block.Column}) - only rockets can damage stones");
                return;
            }

            if (!_state.TryGetValue(block, out var s))
            {
                // Late-damage edge case: ensure we have state.
                s = new StoneState { Health = Mathf.Max(1, maxHealth), Destroyed = false };
            }

            if (s.Destroyed)
            {
                // Already dead; ignore further damage.
                return;
            }

            s.Health -= Mathf.Max(1, damage);

            Debug.Log($"[Stone] Damage {damage} from {source} at ({block.Row},{block.Column}) => HP {s.Health}");

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