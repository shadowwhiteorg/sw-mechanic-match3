using System.Collections;
using System.Collections.Generic;
using _Game.Core.Events;
using _Game.Enums;
using _Game.Systems.BlockSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Vase")]
    public class VaseBehaviorAsset : BlockBehaviorAsset
    {
        [SerializeField] private int maxHealth = 2;
        [SerializeField] private Sprite undamagedSprite;
        [SerializeField] private Sprite damagedSprite;

        // Per-block runtime state lives here (asset is shared; state must not be).
        private readonly Dictionary<BlockModel, VaseState> _state = new();

        private struct VaseState
        {
            public int Health;
            public bool Destroyed;
            public HashSet<int> DamagedByBlastGroups; // Track which blast groups have already damaged this vase
        }

        public override void OnPlaced(BlockModel block)
        {
            base.OnPlaced(block);
            Debug.Log($"[Vase] OnPlaced called for block at ({block.Row},{block.Column})");
            
            _state[block] = new VaseState
            {
                Health = Mathf.Max(2, maxHealth),
                Destroyed = false,
                DamagedByBlastGroups = new HashSet<int>()
            };

            // Set initial sprite (undamaged)
            UpdateVaseSprite(block);

            // TEST: Force damaged sprite after 2 seconds to test sprite system
            CoroutineRunner.Instance.StartCoroutine(TestSpriteChange(block));

            // Optional: debug
            Debug.Log($"[Vase] Placed at ({block.Row},{block.Column}) with {maxHealth} HP");
        }

        public override void OnActivated(BlockModel block)
        {
            base.OnActivated(block);
        }

        public override void OnMatched(BlockModel block)
        {
            // Vases are not cleared by color-matches; only by damage.
        }

        public override void OnFell(BlockModel block)
        {
            // Vases can fall like regular blocks
        }

        public override void OnCleared(BlockModel block)
        {
            // Clean up state when the block is finally removed.
            if (_state.Remove(block))
            {
                // Optional: debug
                Debug.Log($"[Vase] Cleared at ({block.Row},{block.Column}) – state removed");
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
        /// Vases can be damaged by rockets and blasts, but only once per blast group.
        /// </summary>
        public void TakeDamage(int damage, DamageSource source, BlockModel block, int blastGroupId = -1)
        {
            if (block == null) return;
            
            Debug.Log($"[Vase] TakeDamage called for block at ({block.Row},{block.Column}) - damage: {damage}, source: {source}");

            if (!_state.TryGetValue(block, out var s))
            {
                // Late-damage edge case: ensure we have state.
                s = new VaseState 
                { 
                    Health = Mathf.Max(2, maxHealth), 
                    Destroyed = false,
                    DamagedByBlastGroups = new HashSet<int>()
                };
            }

            if (s.Destroyed)
            {
                // Already dead; ignore further damage.
                return;
            }

            // For blast damage, check if we've already been damaged by this blast group
            if (source == DamageSource.Blast && blastGroupId >= 0)
            {
                if (s.DamagedByBlastGroups.Contains(blastGroupId))
                {
                    Debug.Log($"[Vase] Already damaged by blast group {blastGroupId} at ({block.Row},{block.Column}) - ignoring");
                    return;
                }
                s.DamagedByBlastGroups.Add(blastGroupId);
            }

            s.Health -= Mathf.Max(1, damage);

            Debug.Log($"[Vase] Damage {damage} from {source} at ({block.Row},{block.Column}) => HP {s.Health}");

            // Update sprite based on health
            UpdateVaseSprite(block);

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

        private IEnumerator TestSpriteChange(BlockModel block)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log($"[Vase] TEST: Forcing damaged sprite for block at ({block.Row},{block.Column})");
            
            if (_state.TryGetValue(block, out var s))
            {
                s.Health = 1; // Simulate damage
                _state[block] = s;
                UpdateVaseSprite(block);
            }
        }

        private void UpdateVaseSprite(BlockModel block)
        {
            Debug.Log($"[Vase] UpdateVaseSprite called for block at ({block.Row},{block.Column})");
            
            if (!_state.TryGetValue(block, out var s))
            {
                Debug.LogWarning($"[Vase] No state found for block at ({block.Row},{block.Column})");
                return;
            }

            // Determine which sprite to use based on health
            // Use undamaged sprite only when at full health, damaged sprite otherwise
            Sprite targetSprite = s.Health == maxHealth ? undamagedSprite : damagedSprite;
            
            Debug.Log($"[Vase] Sprite selection - HP: {s.Health}/{maxHealth}, undamagedSprite: {undamagedSprite != null}, damagedSprite: {damagedSprite != null}");
            Debug.Log($"[Vase] Target sprite: {targetSprite != null}, block.View: {block.View != null}");
            
            if (targetSprite != null && block.View != null)
            {
                block.View.SetSprite(targetSprite);
                Debug.Log($"[Vase] Updated sprite at ({block.Row},{block.Column}) - HP: {s.Health}/{maxHealth}, Sprite: {(s.Health == maxHealth ? "Undamaged" : "Damaged")}");
            }
            else
            {
                Debug.LogWarning($"[Vase] Failed to update sprite at ({block.Row},{block.Column}) - targetSprite: {targetSprite != null}, block.View: {block.View != null}");
                if (undamagedSprite == null) Debug.LogWarning("[Vase] undamagedSprite is null!");
                if (damagedSprite == null) Debug.LogWarning("[Vase] damagedSprite is null!");
            }
        }
    }
}
