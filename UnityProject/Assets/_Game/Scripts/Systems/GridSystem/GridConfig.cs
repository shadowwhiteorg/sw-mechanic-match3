using UnityEngine;

namespace _Game.Systems.GridSystem
{
    [CreateAssetMenu(menuName = "Configs/GridConfig", fileName = "GridConfig")]
    public class GridConfig : ScriptableObject
    {
        [Min(1)] public int Rows = 8;
        [Min(1)] public int Columns = 8;
        [Min(0.1f)] public float BlockSize = 1f;
        
        public Vector3 GetWorldPosition(int row, int column)
        {
            float xOffset = (Columns - 1) * BlockSize * 0.5f;
            float yOffset = (Rows - 1) * BlockSize * 0.5f;
            float x = column * BlockSize - xOffset;
            float y = -(row * BlockSize - yOffset);
            return new Vector3(x, y, 0f);
        }
    }
}