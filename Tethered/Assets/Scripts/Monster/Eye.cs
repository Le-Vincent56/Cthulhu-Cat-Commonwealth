using System;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using Tethered.Player;
using UnityEngine;

namespace Monster
{
    public class Eye : MonoBehaviour
    {
        [SerializeField] private Transform target;
        
        [Header("Pupil")]
        [SerializeField] private BoxCollider2D boxEye;
        [SerializeField] private Transform eye;
        [SerializeField] private float eyeSpeed = 1f;
        // Multiplier to account for the fact that the eye and target are not on the same plane.
        // Between 0 and 1, closer to 0 the farther the eye is from the target in depth.
        [SerializeField] private float perspectiveMultiplier;
        private Bounds _boxBoundsEye;
        
        /*[Header("Eyelids")]
        [SerializeField] private BoxCollider2D boxEyelid;
        [SerializeField] private Transform eyelids;
        private Bounds _boxBoundsEyelid;*/
        
        void Start()
        {
            _boxBoundsEye = boxEye.bounds;
            //_boxBoundsEyelid = boxEyelid.bounds;
        }
        
        // Takes in a position of the target every frame and uses that to calculate the eye position.
        void Update()
        {
            if (target)
            {
                // Combining MoveTowards and Lerp to create a more organic eye movement.
                var position = Vector2.MoveTowards(_boxBoundsEye.center, target.position, perspectiveMultiplier);
                position = Vector2.Lerp(_boxBoundsEye.center, position, perspectiveMultiplier);
                
                // Check if eye is out of bounds.
                if (!_boxBoundsEye.Contains(position))
                {
                    // Move it into the boundingbox if it is.
                    position = _boxBoundsEye.ClosestPoint(position);
                }

                eye.position = Vector2.Lerp(eye.position, position, eyeSpeed * Time.deltaTime);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // Exit case - if the entity is not a Player Controller
            if (!other.TryGetComponent(out PlayerController controller)) return;

            var head = controller.transform.Find("Vision");

            if (target == null)
            {
                target = head;
                return;
            }

            if (Vector3.Distance(head.position, eye.position) < Vector3.Distance(target.position, eye.position))
            {
                target = head;
            }
        }
        
    }
}
