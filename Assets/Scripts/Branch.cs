using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Branch : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [HideInInspector] public List<Node> nodes = new();
    [HideInInspector] public Node lastNode => nodes.Last();

    public void Initialize(Transform parent, Node rootNode)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        transform.SetParent(parent);
        transform.position = rootNode.position;
        transform.rotation = Quaternion.Euler(rootNode.rotation);

        Node newRootNode = Node.Zero;
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
        meshFilter.mesh = MeshGenerator.GenerateMesh(nodes.ToArray(), sideCount);
    }

    //public void SetRootNode(Node node)
    //{
    //    rootNode = node;
    //    transform.position = node.position;
    //    transform.rotation = Quaternion.Euler(node.rotation);
    //    // meshInfo.GenerateVertex(node, false);
    //}

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
