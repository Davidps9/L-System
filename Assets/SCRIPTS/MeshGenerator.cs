

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List <int> triangles = new List<int>();
    [SerializeField] int sideCount = 4;
    [SerializeField] int radius = 4;
    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    public void GenerateVertex(Vector3 position)
    {     

        vertices.Add(position);
        for (int i = 0; i < sideCount; i++)
        {
            float angle = Mathf.PI * 2 * i / sideCount;
            Vector3 P = new Vector3(radius * Mathf.Sin(angle), position.y, radius * Mathf.Cos(angle));
            vertices.Add(P);    
        }
        int ogIndex = vertices.IndexOf(position);
        for (int i = 0; i < sideCount ; i++)
        {

            triangles.Add(ogIndex);
            triangles.Add(ogIndex + 1 + (i + 1) % sideCount);
            triangles.Add(ogIndex + 1 + i); 
        }
            
    }


    public void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }

  
}