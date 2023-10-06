

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    [SerializeField] int sideCount = 4;
    [SerializeField] float radius = 4;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        AssignDefaultShader();
    }

    public void GenerateVertex(Node node, bool addCap)
    {
        Vector3 angleBetween = Vector3.zero;
        Vector3 localAngleBetween = Vector3.zero;
        if (node.parent != null)
        {
            angleBetween = node.parent.rotation + ((node.rotation - node.parent.rotation) / 2);
            localAngleBetween = (angleBetween - node.parent.rotation);
        }

        Vector2 scale = new Vector2(
            Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad),
            Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad)
        );

        //Debug.Log("local rotation: " + node.localRotation + ", scale: " + scale);

        vertices.Add(node.position);
        int nodeIndex = vertices.IndexOf(node.position);

        Quaternion rotation = Quaternion.Euler(angleBetween.x, 0, angleBetween.z);
        for (int i = 0; i < sideCount; i++)
        {
            float baseAngle = Mathf.PI * 2 * i / sideCount;

            Vector3 vertex = new Vector3(radius * Mathf.Sin(baseAngle) * scale.y, 0, radius * Mathf.Cos(baseAngle) * scale.x);
            vertex = rotation * vertex;
            vertex += node.position;
            vertices.Add(vertex);
        }

        if (addCap)
        {
            for (int i = 0; i < sideCount; i++)
            {
                triangles.Add(nodeIndex);
                triangles.Add(nodeIndex + 1 + (i + 1) % sideCount);
                triangles.Add(nodeIndex + 1 + i);
            }
        }

        if (node.parent != null)
        {
            int parentIndex = vertices.IndexOf(node.parent.position);
            CreateWalls(nodeIndex, parentIndex);
        }
    }


    public void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        //Debug.Log(vertices.Count);
    }

    private void CreateWalls(int index, int prevIndex)
    {
        int size = sideCount;
        int bottomFirstIndex = prevIndex + 1;
        int topFirstIndex = index + 1;

        for (int i = 0; i < size; i++)
        {
            int indexer = 1;
            if (i == size - 1)
            {
                indexer = -(size - 1);
            }

            triangles.Add(topFirstIndex + i);
            triangles.Add(bottomFirstIndex + i);
            triangles.Add(bottomFirstIndex + indexer + i);

            triangles.Add(topFirstIndex + i);
            triangles.Add(bottomFirstIndex + indexer + i);
            triangles.Add(topFirstIndex + indexer + i);
        }
    }

    public void ResetMesh()
    {
        vertices.Clear();
        triangles.Clear();
    }

    public void AssignDefaultShader()
    {
        // white Diffuse shader, better than the default magenta
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
        meshRenderer.sharedMaterial.color = Color.white;
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices.Count > 0)
    //    {
    //        Gizmos.color = Color.red;
    //        for (int i = 0; i < vertices.Count; i++)
    //        {
    //            Gizmos.DrawSphere(vertices[i], 0.1f);
    //        }
    //    }
    //}
}