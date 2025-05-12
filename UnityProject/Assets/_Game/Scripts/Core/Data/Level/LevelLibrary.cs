using System.Collections.Generic;
using UnityEngine;

namespace _Game.Core.Data
{
    [CreateAssetMenu(menuName = "Game/Level Library", fileName = "LevelLibrary")]
    public class LevelLibrary : ScriptableObject
    {
        [Tooltip("Assign each level's JSON file imported as a TextAsset")]
        public List<TextAsset> levelJsonAssets = new List<TextAsset>();
    }
}