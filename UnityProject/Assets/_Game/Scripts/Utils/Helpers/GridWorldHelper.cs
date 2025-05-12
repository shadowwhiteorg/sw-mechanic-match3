using UnityEngine;
using _Game.Data;
using _Game.Systems.GridSystem;

namespace _Game.Utils
{
    public class GridWorldHelper
    {
        private readonly GridConfig _config;
        private readonly Vector3 _origin; // bottom-left of grid in world space
        public GridConfig GridConfig => _config;

        public GridWorldHelper(GridConfig config, Vector3 origin)
        {
            _config = config;
            _origin = origin;
            float xOffset = (config.Columns ) * config.BlockSize * 0.5f;
            float yOffset = (config.Rows ) * config.BlockSize * 0.5f;
            _origin.x -= xOffset;
            _origin.y += yOffset;
        }

        public Vector3 GetWorldPosition(int row, int col)
        {
            float x = _origin.x + col * _config.BlockSize + _config.BlockSize * 0.5f;
            float y = _origin.y - row * _config.BlockSize - _config.BlockSize * 0.5f;
            float z = _origin.z + row * 0.1f;
            return new Vector3(x, y, z);
        }

        public bool TryGetGridPosition(Vector3 worldPos, out int row, out int col)
        {
            float localX = worldPos.x - _origin.x;
            float localY = _origin.y - worldPos.y; // inverted
            col = Mathf.FloorToInt(localX / _config.BlockSize);
            row = Mathf.FloorToInt(localY / _config.BlockSize);
            if (row >= 0 && row < _config.Rows && col >= 0 && col < _config.Columns)
                return true;
            row = col = -1;
            return false;
        }
    }
}