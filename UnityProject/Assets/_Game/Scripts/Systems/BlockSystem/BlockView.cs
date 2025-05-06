using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Systems.BlockSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        /// <summary>Sets the block’s sprite.</summary>
        public void SetSprite(Sprite sprite)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = sprite;
        }

        /// <summary>Moves the view to the given world position.</summary>
        public void SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }
        
        private Vector3 GetWorldPosition()
        {
            return transform.position;
        }
    }
}