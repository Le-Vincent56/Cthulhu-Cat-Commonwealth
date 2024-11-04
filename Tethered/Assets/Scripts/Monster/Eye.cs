using UnityEngine;

namespace Monster
{
    public class Eye : MonoBehaviour
    {
        [SerializeField] private Transform target;
        
        [Header("Pupil")]
        [SerializeField] private BoxCollider2D boxEye;
        [SerializeField] private Transform eye;
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
            // Combining MoveTowards and Lerp to create a more organic eye movement.
            eye.position = Vector2.MoveTowards(_boxBoundsEye.center, target.position, perspectiveMultiplier);
            eye.position = Vector2.Lerp(_boxBoundsEye.center, eye.position, perspectiveMultiplier);
            
            // Check if eye is out of bounds.
            if (!_boxBoundsEye.Contains(eye.position))
            {
                // Move it into the boundingbox if it is.
                eye.position = _boxBoundsEye.ClosestPoint(eye.position);
            }
        }
    }
}
