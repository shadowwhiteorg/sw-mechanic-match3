using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(menuName = "Configs/MatchConfig")]
    public class MatchConfig : ScriptableObject
    {
        public int MinMatchSize = 2;
    }
}