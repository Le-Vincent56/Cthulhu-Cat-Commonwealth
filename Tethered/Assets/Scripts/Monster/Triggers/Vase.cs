using DG.Tweening;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Monster.Triggers
{
    public class Vase : AttractionTrigger
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float pushForce;

        private Tween rotateTween;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            Callback(collision);
        }

        private void OnDestroy()
        {
            // Kill the rotate tween
            rotateTween?.Kill();
        }

        /// <summary>
        /// Knock over the vase as a callback
        /// </summary>
        private void Callback(Collider2D collision)
        {
            // Exit case - if the entity is not a Player Controller
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Get the direction of the collision
            Vector2 collisionDirection = (controller.transform.position - transform.position).normalized;
            float pushDirection = -Mathf.Sign(collisionDirection.x);
            Vector2 totalForce = new Vector2(pushDirection * pushForce, 0);

            rb.AddForce(totalForce);
            rb.gravityScale = 0.5f;
            rotateTween = rb.DORotate(-pushDirection * 45f, 0.2f);
        }
    }
}