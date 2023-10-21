using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Branch : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    [HideInInspector] public List<Node> nodes = new();
    [HideInInspector] public Node lastNode => nodes.Last();

    #region Branch Creation

    public void Initialize(Transform parent, Node rootNode)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        transform.SetParent(parent);
        transform.localPosition = rootNode.position;

        Node newRootNode = new(Vector3.zero, rootNode.rotation);
        newRootNode.radius = rootNode.radius;
        ApplyNode(newRootNode);
    }

    public void CreateMesh(int sideCount, Material material)
    {
        if (nodes.Count < 1)
        {
            Debug.LogError("Not enough nodes to create a mesh");
            return;
        }

        meshRenderer.material = material;
        Mesh mesh = MeshGenerator.GenerateMesh(nodes.ToArray(), sideCount, "Branch");
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public Node CreateNode(float radius)
    {
        Node node = new(lastNode, true, radius);
        // nodes.Add(node);
        return node;
    }

    public void ApplyNode(Node node)
    {
        nodes.Add(node);
    }

    #endregion

    #region Branch Interaction

    [Header("Movement")]
    [SerializeField] private float maxAngle = 5;
    [SerializeField] private float maxVelocity = 10;
    [SerializeField] private float damping = 0.99f;
    [SerializeField] private float forceMultiplier = 100;

    private float angle = 0.0f;          // Current swing angle (in radians)
    private float angularVelocity = 0.0f; // Angular velocity

    void Update()
    {
        // Simulate damping
        angularVelocity *= damping;

        // Calculate the swing motion
        angle += angularVelocity * Time.deltaTime;

        RotateZ(Mathf.Sin(angle), maxAngle);
    }

    // Apply a force to the swing
    public void ApplyForce(Vector2 force)
    {
        // Calculate the angular acceleration
        float angularAcceleration = force.x;

        // Check the direction of the force relative to the current swing direction
        // Increase or decrease the angular velocity accordingly
        if (Vector2.Dot(force.normalized, new Vector2(Mathf.Sin(angle), -Mathf.Cos(angle))) > 0)
        {
            angularVelocity += angularAcceleration * Time.deltaTime * forceMultiplier;
        }
        else
        {
            angularVelocity -= angularAcceleration * Time.deltaTime * forceMultiplier;
        }

        //Debug.Log($"Force {angularVelocity}");
        if (Mathf.Abs(angularVelocity) > maxVelocity)
        {
            angularVelocity = Mathf.Sign(angularVelocity) * maxVelocity;
        }
    }

    private void RotateZ(float normalizedAngle, float maxAngle)
    {
        //Debug.Log($"Angle: {maxAngle}");
        transform.eulerAngles = new Vector3(0, 0, normalizedAngle * maxAngle);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Fish>(out var fish))
        {
            fish.AvoidPoint(transform.position, 0.5f);
        }
    }

    #endregion
}
