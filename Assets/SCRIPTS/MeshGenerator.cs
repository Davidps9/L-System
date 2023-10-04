

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

    List<Vector3> vertices = new List<Vector3>();
    List <int> triangles = new List<int>();
    [SerializeField] int vertexDepth = 4;
    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    public void GenerateVertex(Vector3 position)
    {     


        vertices.Add(position);
        vertices.Add(position + Vector3.right);
        vertices.Add(position + Vector3.back);
        vertices.Add(position + Vector3.back + Vector3.right);

        if (vertices.Count > vertexDepth)
        {

            AddBase(vertices.IndexOf(position));

            //for (int i = 0; i < vertexDepth; i++)
            //{
                AddWall(vertices.IndexOf(position), 0);
                AddWall(vertices.IndexOf(position), 1);
               
            //}




        }
        else
        {
            AddBase(vertices.IndexOf(position));
        }
       
    }

    private void AddWall(int index,int offset)
    {
        int res = 1 + offset;
        triangles.Add(index  + offset);
        triangles.Add(index - (4 - offset));
        triangles.Add(index - (3 + offset));
        triangles.Add(index + res);
        triangles.Add(index + offset);
        triangles.Add(index - (3 + offset));
    }

    public void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void AddBase(int index)
    {
        triangles.Add(index);
        triangles.Add(index + 2);
        triangles.Add(index + 3);
        triangles.Add(index);
        triangles.Add(index + 3);
        triangles.Add(index + 1);
    }
}