using System.Collections;
using System.Collections.Generic;
using Tethered.Patterns.ServiceLocator;
using Tethered.Player;
using UnityEngine;

namespace Tethered.Cameras
{
    public class CameraBoundary : MonoBehaviour
    {
        private Camera mainCamera;
        private List<PlayerController> players;

        private void Awake()
        {
            // Get the main camera
            mainCamera = Camera.main;

            // Initialize the Player List
            players = new();

            // Register this as a service
            ServiceLocator.ForSceneOf(this).Register(this);
        }

        private void LateUpdate()
        {
            // Iterate through each Player
            foreach(PlayerController player in players)
            {
                // Bind the Player's movement
                BindPlayerMovement(player.transform, player.GetComponent<BoxCollider2D>());
            }
        }

        /// <summary>
        /// Get the current bounds of the Main Camera
        /// </summary>
        private void GetCameraBounds(out Vector2 minBounds, out Vector2 maxBounds)
        {
            Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            minBounds = new Vector2(bottomLeft.x, bottomLeft.y);
            maxBounds = new Vector2(topRight.x, topRight.y);
        }

        /// <summary>
        /// Bind player movement to within the boundary
        /// </summary>
        public void BindPlayerMovement(Transform player, BoxCollider2D playerCollider)
        {
            // Retrieve the camera bounds
            GetCameraBounds(out Vector2 minBounds, out Vector2 maxBounds);

            // Get player collider bounds
            Bounds colliderBounds = playerCollider.bounds;

            // Convert player local position to world position
            Vector3 worldPosition = player.parent.TransformPoint(player.localPosition);

            // Clamp the player's position so the collider stays within camera bounds
            worldPosition.x = Mathf.Clamp(
                worldPosition.x, 
                minBounds.x + (colliderBounds.extents.x), 
                maxBounds.x - (colliderBounds.extents.x)
            );
            worldPosition.y = Mathf.Clamp(
                worldPosition.y, 
                minBounds.y + (colliderBounds.extents.y), 
                maxBounds.y - (colliderBounds.extents.y)
            );

            // Conver the world position back to local space
            player.localPosition = player.parent.InverseTransformPoint(worldPosition);
        }

        /// <summary>
        /// Register a Player to be included within the Camera Boundary
        /// </summary>
        public void Register(PlayerController player) => players.Add(player);
    }
}