using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Branch : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [HideInInspector] public List<Node> nodes = new();
    [HideInInspector] public Node lastNode => nodes.Last();
    [HideInInspector] public MeshInfo meshInfo = new();

    public void Initialize(Transform parent, Node rootNode, int sideCount, float radius, Material material)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        meshInfo.sideCount = sideCount;
        meshInfo.radius = radius;
        meshRenderer.material = material;

        transform.SetParent(parent);
        transform.position = rootNode.position;
        transform.rotation = Quaternion.Euler(rootNode.rotation);
        ApplyNode(Node.Zero);
    }

    public void CreateMesh()
    {
        if (nodes.Count < 1)
        {
            Debug.LogError("Not enough nodes to create a mesh");
            return;
        }

        meshInfo.GenerateMesh(nodes.ToArray());

        Mesh mesh = new();
        mesh.SetVertices(meshInfo.vertices);
        mesh.SetTriangles(meshInfo.triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        Debug.Log(mesh.vertices.Length);
        Unwrapping.GenerateSecondaryUVSet(mesh);

        meshFilter.mesh = mesh;
    }

    //public void SetRootNode(Node node)
    //{
    //    rootNode = node;
    //    transform.position = node.position;
    //    transform.rotation = Quaternion.Euler(node.rotation);
    //    // meshInfo.GenerateVertex(node, false);
    //}

    public Node CreateNode()
    {
        Node node = new(lastNode, true);
        // nodes.Add(node);
        return node;
    }

    public void ApplyNode(Node node)
    {
        nodes.Add(node);
    }

    private void OnDrawGizmos()
    {
        if (nodes.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < nodes.Count; i++)
            {
                Gizmos.DrawSphere(nodes[i].position, 0.1f);
                Gizmos.DrawMesh(meshFilter.mesh);
            }
        }
    }
}
