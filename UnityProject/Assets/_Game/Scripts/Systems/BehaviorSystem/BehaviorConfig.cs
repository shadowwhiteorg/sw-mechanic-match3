using UnityEngine;

namespace _Game.Systems.BehaviorSystem
{
    [CreateAssetMenu(menuName="Blast/Behavior Config", fileName="BehaviorConfig")]
    public class BehaviorConfig : ScriptableObject
    {
        [Tooltip("All special-block behavior assets in your game")]
        public BlockBehaviorAsset[] behaviors;
    }
}