

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();
    List <int> triangles = new List<int>();

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    public void GenerateVertex(Vector3 position)
    {
        vertices.Add(position);
        vertices.Add(position + Vector3.right);
        vertices.Add(position + Vector3.forward);
        vertices.Add(position + Vector3.forward + Vector3.right);
        if (vertices.Count > 4)
        {

            triangles.Add(vertices.IndexOf(position) + 2);
            triangles.Add(vertices.IndexOf(position));
            triangles.Add(vertices.IndexOf(position) + 1);
            triangles.Add(vertices.IndexOf(position) + 2);
            triangles.Add(vertices.IndexOf(position) + 3);
            triangles.Add(vertices.IndexOf(position) + 1);
            triangles.Add(vertices.IndexOf(position));
            triangles.Add(vertices.IndexOf(position) - 4 );
            triangles.Add(vertices.IndexOf(position) - 3);
            triangles.Add(vertices.IndexOf(position) + 1);
            triangles.Add(vertices.IndexOf(position));            
            triangles.Add(vertices.IndexOf(position) - 3);
        }
        else
        {
            triangles.Add(vertices.IndexOf(position));
            triangles.Add(vertices.IndexOf(position) + 1);
            triangles.Add(vertices.IndexOf(position) + 2);
            triangles.Add(vertices.IndexOf(position) + 2);            
            triangles.Add(vertices.IndexOf(position) + 3);
            triangles.Add(vertices.IndexOf(position) + 1);
        }
       
    }


    public void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}