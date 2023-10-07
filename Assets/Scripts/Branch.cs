using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Branch : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [HideInInspector] public Node rootNode;
    [HideInInspector] public MeshInfo meshInfo = new();

    public void CreateMesh()
    {
        if (meshInfo.vertices.Count < 3)
        {
            Debug.LogError("Not enough vertices to create a mesh");
            return;
        }
        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(meshInfo.vertices);
        mesh.SetTriangles(meshInfo.triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        Debug.Log(mesh.vertices.Length);
        Unwrapping.GenerateSecondaryUVSet(mesh);

        meshFilter.mesh = mesh;
    }

    public void SetRootNode(Node node)
    {
        rootNode = node;
        transform.position = node.position;
        transform.rotation = Quaternion.Euler(node.rotation);
    }

    public void SetRenderParameters(int sideCount, float radius, Material material)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        meshInfo.sideCount = sideCount;
        meshInfo.radius = radius;
        meshRenderer.material = material;
    }

    private void OnDrawGizmos()
    {
        if (meshInfo.vertices.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < meshInfo.vertices.Count; i++)
            {
                Gizmos.DrawSphere(meshInfo.vertices[i], 0.1f);
                Gizmos.DrawMesh(meshFilter.mesh);
            }
        }
    }
}
