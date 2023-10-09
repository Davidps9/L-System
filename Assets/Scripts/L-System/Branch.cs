using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Joint))]
[RequireComponent(typeof(MeshCollider))]
public class Branch : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Rigidbody rb;
    private Joint joint;
    private MeshCollider meshCollider;
    [HideInInspector] public List<Node> nodes = new();
    [HideInInspector] public Node lastNode => nodes.Last();

    public void Initialize(Transform parent, Node rootNode)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<Joint>();
        meshCollider = GetComponent<MeshCollider>();

        transform.SetParent(parent, false);

        if (parent.TryGetComponent<Rigidbody>(out var parentRb))
        {
            transform.localPosition = rootNode.position;
            joint.connectedBody = parentRb;
            joint.autoConfigureConnectedAnchor = true;
        }
        else
        {
            joint.connectedAnchor = transform.position;
        }

        Node newRootNode = new Node(Vector3.zero, rootNode.rotation);
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

        rb.isKinematic = false;
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

    //private void OnDrawGizmos()
    //{
    //    if (nodes.Count > 0)
    //    {
    //        Gizmos.color = Color.red;
    //        for (int i = 0; i < nodes.Count; i++)
    //        {
    //            Gizmos.DrawSphere(nodes[i].position, 0.1f);
    //            Gizmos.DrawMesh(meshFilter.mesh);
    //        }
    //    }
    //}
}
