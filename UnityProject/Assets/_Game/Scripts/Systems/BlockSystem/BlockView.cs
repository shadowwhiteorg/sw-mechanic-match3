using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Systems.BlockSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetSprite(Sprite sprite)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = sprite;
        }

        public void SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }
    }
}