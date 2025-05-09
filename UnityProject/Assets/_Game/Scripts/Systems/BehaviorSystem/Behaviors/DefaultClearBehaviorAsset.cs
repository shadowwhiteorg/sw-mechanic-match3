using _Game.Core.Events;
using _Game.Enums;
using _Game.Interfaces;
using _Game.Systems.BlockSystem;
using _Game.Systems.GridSystem;
using _Game.Utils;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Default Clear")]
    public class DefaultClearBehaviorAsset : BlockBehaviorAsset
    {
        // When a normal block is matched in a group, clear it:
        public override void OnMatched(BlockModel block)
        {
            Events.Fire(new ClearBlockEvent(block));
        }

        // optional VFX/SFX on clear
        public override void OnCleared(BlockModel block)
        {
            // e.g. Instantiate(clearParticles, block.View.transform.position, Quaternion.identity);
        }
    }
}