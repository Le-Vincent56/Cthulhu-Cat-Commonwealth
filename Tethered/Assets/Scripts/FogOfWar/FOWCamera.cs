using UnityEngine;

namespace Tethered.FogOfWar
{
    /// <summary>
    /// Sets up Fog of War camera in the context of the main camera.
    /// </summary>
    public class FOWCamera : MonoBehaviour
    {
        private Camera _cam;
        
        // Set up Fog of War Camera, Render Texture, and Material Plane
        void Start()
        {
            _cam = GetComponent<Camera>();
            // assumes main camera exists
            if (Camera.main != null)
            {
                var main = Camera.main;
                // currently assumes only one fog of war camera and one render texture for it, not scalable.
                // TODO: Make camera and render texture at runtime.
                // Instantiating at runtime will help with different aspect ratios and if we need multiple cameras
                // in the future.
                
                // Parent camera to main camera and match dimensions.
                var targetTexture = _cam.targetTexture;
                transform.parent = main.transform; 
                targetTexture.height = main.pixelHeight;
                targetTexture.width = main.pixelWidth;
                
                // Create a plane right in front of the main camera
                // TODO: Make sprite generation and scaling with camera more procedural.
                // TODO: Improve performance. Compute shaer?
                
                // Currently uses R channel for alpha masking.
                // Layer 12 - FogOfWar used for this camera.
            }
            else
            {
                Debug.LogError("Main Camera does not exist!");
            }
        }
    }
}
