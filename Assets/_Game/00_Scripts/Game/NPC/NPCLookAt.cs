using UnityEngine;

namespace Slafurry.Game.NPC
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class NPCLookAt : MonoBehaviour
    {
        [Header("Look At")]
        [Tooltip("Which way this sprite is drawn by default. Uncheck if your NPC sprite is drawn facing left.")]
        [SerializeField] private bool spriteFacesRight = true;

        [SerializeField] private SpriteRenderer spriteRenderer;

        private bool isFacingRight = true;

        public bool IsFacingRight => isFacingRight;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        /// <summary>
        /// Flip this NPC left or right so it faces the given target.
        /// Pure 2D flip — no rotation.
        /// </summary>
        public void LookAt(Transform target)
        {
            if (target == null || spriteRenderer == null) return;

            LookAtPosition(target.position);
        }

        /// <summary>
        /// Flip this NPC left or right so it faces a world-space point.
        /// Pure 2D flip — no rotation.
        /// </summary>
        public void LookAt(Vector2 worldPosition)
        {
            LookAtPosition(worldPosition);
        }

        private void LookAtPosition(Vector2 targetPosition)
        {
            if (spriteRenderer == null) return;

            Vector2 direction = targetPosition - (Vector2)transform.position;

            if (direction.sqrMagnitude < 0.0001f) return;

            bool targetIsRight = direction.x > 0f;

            // spriteFacesRight = true : face right target => flipX = false
            // spriteFacesRight = false: face right target => flipX = true  (sprite drawn facing left)
            bool flipX = spriteFacesRight ? !targetIsRight : targetIsRight;

            spriteRenderer.flipX = flipX;
            isFacingRight = !flipX;
        }
    }
}
