using System;
using System.Collections.Generic;
using _Game.Enums;
using UnityEngine;

namespace _Game.Systems.GameLoop
{
    [CreateAssetMenu(menuName = "Blast/Level Data", fileName = "LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("Grid")]
        [Min(1)] public int Rows = 8;
        [Min(1)] public int Columns = 8;

        [Header("Gameplay")]
        [Tooltip("How many moves the player gets")]
        public int MoveLimit = 20;

        [Header("Goals")]
        public List<ColorGoal>   ColorGoals = new List<ColorGoal>();
        public List<TypeGoal>    TypeGoals  = new List<TypeGoal>();

        [Header("Initial Layout")]
        [Tooltip("Row-major, length = Rows*Columns")]
        public List<BlockDefinition> InitialBlocks = new List<BlockDefinition>();

        private void OnValidate()
        {
            // ensure InitialBlocks has exactly Rows*Columns elements
            int target = Rows * Columns;
            while (InitialBlocks.Count < target)
                InitialBlocks.Add(new BlockDefinition());
            while (InitialBlocks.Count > target)
                InitialBlocks.RemoveAt(InitialBlocks.Count - 1);
        }

        public void OnValidateTrigger()
        {
            OnValidate();
        }

        [Serializable]
        public struct ColorGoal
        {
            public BlockColor Color;
            public int        Count;
        }

        [Serializable]
        public struct TypeGoal
        {
            public BlockType Type;
            public int       Count;
        }

        [Serializable]
        public struct BlockDefinition
        {
            public BlockColor Color;
            public BlockType  Type;
            public BlockDirection Direction;
        }
    }
}