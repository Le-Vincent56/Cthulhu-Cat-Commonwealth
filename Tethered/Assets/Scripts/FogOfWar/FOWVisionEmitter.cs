using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vision emitters manage the shape and occlusion of the fog of war vision objects.
/// They handle when vision needs to be updated.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class FOWVisionEmitter : MonoBehaviour
{
    // Fields
    // Sprite used to render the fog of war
    [Header("Visualization")] [SerializeField]
    private Sprite visionMask;

    [SerializeField] private Material visionMaterial;

    [Header("Functionality")] [SerializeField]
    private float visionRadius;

    [SerializeField] private VisionEmitterMode mode;

    // TODO: Do we need this or implement purely in shader?
    // Layers to check for vision targets for objects you want to disappear and reappear with vision instead of just
    // fading in the background.
    [SerializeField] private LayerMask targetMask;

    // Layers to check for blocking vision.
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private List<Transform> visibleTargets = new List<Transform>();
    
    // Properties
    public float VisionRadius
    {
        get => visionRadius;
    }

    public List<Transform> VisibleTargets
    {
        get => visibleTargets;
    }

    /// <summary>
    /// Get all targets in view radius non obstructed by colliders.
    /// </summary>
    void FindVisibleTarget()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, visionRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            var lineToTarget = targetsInViewRadius[i].transform.position - transform.position;
            
            if (!Physics2D.Raycast(transform.position, lineToTarget.normalized,
                    lineToTarget.magnitude, obstacleMask))
            {
                visibleTargets.Add(targetsInViewRadius[i].transform);
            }
        }
    }

    /// <summary>
    /// Get a directional vector on a 2D circle given an angle. 0 degrees is up, 90 degrees is right, etc. 
    /// </summary>
    /// <param name="angleDegrees"></param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float angleDegrees)
    {
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }

    private void FixedUpdate()
    {
        if (mode == VisionEmitterMode.Realtime)
        {
            FindVisibleTarget();
        }
    }
}

// TODO: Implement Different Modes
// - Realtime
// - Static
// - Stationary
public enum VisionEmitterMode
{
    Realtime,
    Static,
    Stationary
}