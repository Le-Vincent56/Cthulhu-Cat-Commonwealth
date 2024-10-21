using Tethered.Interactables;
using UnityEngine;

namespace Tethered.Level
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PushableTrigger : MonoBehaviour
    {
        [SerializeField] private float lockDistance;

        private void OnTriggerStay2D(Collider2D other)
        {
            // Exit case - if the entering collider is not a Pushable
            if (!other.TryGetComponent(out Moveable pushable)) return;

            // Exit case - if not within locking distance
            if (Vector3.Distance(transform.position, other.transform.position) > lockDistance) return;

            // Lock the pushable onto the center of the trigger
            pushable.LockIntoPlace(transform.position);
        }
    }
}