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
        public override BlockType Type => BlockType.None;

        private IEventBus _eventBus;
        
        public override void OnMatched(BlockModel block)
        {
            Events.Fire(new ClearBlockEvent(block));
        }

        public override void OnCleared(BlockModel block)
        {
            // You can add VFX or SFX here later if needed
        }
    }
}