using UnityEngine;
namespace Tethered.World
{
    public class Destructor : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Exit case - the collider has no Destructable component
            if (!collision.gameObject.TryGetComponent(out Destructable destructable)) return;

            // Destruct the destructable
            destructable.Destruct();
        }
    }
}
