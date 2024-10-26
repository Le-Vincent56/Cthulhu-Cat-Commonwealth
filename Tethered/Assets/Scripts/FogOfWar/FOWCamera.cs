using System;
using UnityEngine;

namespace Tethered.FogOfWar
{
    /// <summary>
    /// Sets up Fog of War camera in the context of the main camera.
    /// </summary>
    public class FOWCamera : MonoBehaviour
    {
        private Camera _cam;
        private Camera _mainCam;
        [SerializeField] private GameObject fogOfWar;
        
        // Set up Fog of War Camera, Render Texture, and Material Plane
        void Start()
        {
            fogOfWar.SetActive(true);
            _cam = GetComponent<Camera>();
            // assumes main camera exists
            if (Camera.main != null)
            {
                _mainCam = Camera.main;
                // currently assumes only one fog of war camera and one render texture for it, not scalable.
                // Instantiating at runtime will help with different aspect ratios and if we need multiple cameras
                // in the future.
                
                // Parent camera to main camera and match dimensions.
                //var targetTexture = _cam.targetTexture;
                //transform.parent = _mainCam.transform; 
                //targetTexture.height = _mainCam.pixelHeight;
                //targetTexture.width = _mainCam.pixelWidth;
                
                // Create a plane right in front of the main camera
                // TODO: Improve performance. Compute shader?
                
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
