// Reference: https://youtu.be/rQG9aUWarwE?si=sunQ6o1e5iL6fJE0
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tethered.Monster.Events;
using Tethered.Patterns.EventBus;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Vision emitters manage the shape and occlusion of the fog of war vision objects.
/// They handle when vision needs to be updated.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FOWVisionEmitter : MonoBehaviour
{
    // Fields
    
    [Header("Visualization")]
    // pass in radius to calculate in object space position maximum light radius
    // why can use power to determine light fall off
    [SerializeField] private Material visionMaterial;

    [Header("Functionality")]
    [SerializeField] private float visionRadius = 1f;
    [SerializeField] private float minVisionRadius = 0f;
    [SerializeField] private float maxVisionRadius = 4f;
    [SerializeField] private float visionStrength;
    
    // Multiplier for difference between target detection and vision calculations
    [SerializeField] private float radiusTargetFactor = 1f;

    // How many rays to use when calculating occlusion.
    [SerializeField] private int visionResolution = 36;
    [SerializeField] private int edgeResolution = 8;

    [SerializeField] private VisionEmitterMode mode;
    
    // Layers to check for vision targets for objects you want to disappear and reappear with vision instead of just
    // fading in the background.
    [SerializeField] private LayerMask targetMask;

    // Layers to check for blocking vision.
    [SerializeField] private LayerMask obstacleMask;

    private List<Transform> _visibleTargets = new List<Transform>();
    private MeshRenderer _visionMeshRenderer;
    private MeshFilter _visionMeshFilter;
    private Mesh _visionMesh;
    private static readonly int Radius = Shader.PropertyToID("_Radius");

    private EventBinding<AttractionChanged> onAttractionChanged;

    // Properties
    public float VisionRadius
    {
        get => visionRadius;
    }

    public List<Transform> VisibleTargets
    {
        get => _visibleTargets;
    }

    private float VisionStrength
    {
        get => visionStrength;
        set
        {
            visionStrength = value;
            visionRadius = (maxVisionRadius - minVisionRadius) * value + minVisionRadius;
            visionRadius = Mathf.Clamp(visionRadius, minVisionRadius, maxVisionRadius);
        }
    }

    private void Start()
    {
        _visionMeshFilter = GetComponent<MeshFilter>();
        
        _visionMesh = new Mesh();
        _visionMesh.name = "Vision Mesh";
        _visionMeshFilter.mesh = _visionMesh;

        _visionMeshRenderer = GetComponent<MeshRenderer>();
        _visionMeshRenderer.material = visionMaterial;
    }

    private void OnEnable()
    {
        onAttractionChanged = new EventBinding<AttractionChanged>(OnAttractionChanged);
        EventBus<AttractionChanged>.Register(onAttractionChanged);
    }

    private void OnDisable()
    {
        EventBus<AttractionChanged>.Deregister(onAttractionChanged);
    }

    void FixedUpdate()
    {
        if (mode == VisionEmitterMode.Realtime)
        {
            FindVisibleTarget();
        }
    }

    void LateUpdate()
    {
        visionMaterial.SetFloat(Radius, visionRadius);
        DrawVision();
    }


    /// <summary>
    /// Get all targets in view radius non obstructed by colliders.
    /// </summary>
    void FindVisibleTarget()
    {
        _visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, visionRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            var lineToTarget = targetsInViewRadius[i].transform.position - transform.position;
            
            if (!Physics2D.Raycast(transform.position, lineToTarget.normalized,
                    lineToTarget.magnitude, obstacleMask))
            {
                _visibleTargets.Add(targetsInViewRadius[i].transform);
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

    private void DrawVision()
    {
        float stepSize = 360f / visionResolution;

        List<Vector3> viewPoints = new List<Vector3>();
        VisionCastInfo oldVision = new VisionCastInfo();
        
        // Cast rays in 360 degrees
        for (int i = 0; i < visionResolution; i++)
        {
            // angle to cast ray at
            float angle = stepSize * i;
            VisionCastInfo visionInfo = VisionCast(angle);

            if (i > 0)
            {
                // Find the edge of an occluder if one ray is obstructed and the other isn't
                if (oldVision.IsHit != visionInfo.IsHit || oldVision.Collision != visionInfo.Collision)
                {
                    EdgeInfo edge = FindEdge(oldVision, visionInfo);
                    if (edge.NearPoint != Vector3.zero)
                    {
                        viewPoints.Add(edge.NearPoint);
                    }
                    if (edge.FarPoint != Vector3.zero)
                    {
                        viewPoints.Add(edge.FarPoint);
                    }
                }
            }
            
            viewPoints.Add(visionInfo.Point);
            oldVision = visionInfo;
        }

        // Arrays needed to create a mesh in script
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 1) * 3];

        // Iterates over data from ray casts and assigns them to appropriate indices to create a "fan" shaped mesh
        // Vertex positions should all be in object space.
        vertices[0] = Vector2.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Sets the last triangle to complete a full circle;
        triangles[^3] = 0;
        triangles[^2] = vertexCount - 1;
        triangles[^1] = 1;

        // Create the mesh new mesh
        _visionMesh.Clear();
        _visionMesh.vertices = vertices;
        _visionMesh.triangles = triangles;
        _visionMesh.RecalculateNormals();
    }
    
    VisionCastInfo VisionCast(float angle)
    {
        return VisionCast(angle, visionRadius);
    }

    VisionCastInfo VisionCast(float angle, float visionRadius)
    {
        Vector3 dir = DirFromAngle(angle);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, visionRadius, obstacleMask);

        if (hit)
        {
            return new VisionCastInfo(true, hit.point, hit.distance, angle, hit.collider);
        }
        
        return new VisionCastInfo(false, transform.position + dir * visionRadius, visionRadius, angle, null);
    }

    EdgeInfo FindEdge(VisionCastInfo min, VisionCastInfo max)
    {
        float minAngle = min.Angle;
        float maxAngle = max.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolution; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            VisionCastInfo newVisionCast = VisionCast(angle);

            if (newVisionCast.IsHit == min.IsHit)
            {
                minAngle = angle;
                minPoint = newVisionCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newVisionCast.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private void OnAttractionChanged(AttractionChanged eventData)
    {
        var ratio = (eventData.AttractionLevelMax - eventData.AttractionLevelTotal) / eventData.AttractionLevelMax;
        
        // change tween to tweening a separate variable, for better consolidation, fewer Tweens running.
        DOTween.To(() => VisionStrength, x => VisionStrength = x, ratio, 1);
    }

    public struct VisionCastInfo
    {
        public bool IsHit;
        public Vector3 Point;
        public float Dst;
        public float Angle;
        public Collider2D Collision;

        public VisionCastInfo(bool hit, Vector3 point, float dst, float angle, Collider2D collision)
        {
            IsHit = hit;
            Point = point;
            Dst = dst;
            Angle = angle;
            Collision = collision;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 NearPoint;
        public Vector3 FarPoint;

        public EdgeInfo(Vector3 nearPoint, Vector3 farPoint)
        {
            NearPoint = nearPoint;
            FarPoint = farPoint;
        }
    }
}

// TODO: Implement Different Modes
// - Realtime
// - Static
// - Stationary
public enum VisionEmitterMode
{
    Realtime, // Updates every frame or close to
    Static, // Updates once at the start and never again, bake?
    Stationary, // Updates only when something when an obstacle in its range moves.
    Unoccluded, // Does not have a mesh that needs to be calculated.
}