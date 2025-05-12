using _Game.Core.Events;
using _Game.Systems.BlockSystem;
using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName = "Blast/Behaviors/Default Clear")]
    public class DefaultClearBehaviorAsset : BlockBehaviorAsset
    {
        public override void OnMatched(BlockModel block)
        {
            Events.Fire(new ClearBlockEvent(block));
        }
        
    }
}