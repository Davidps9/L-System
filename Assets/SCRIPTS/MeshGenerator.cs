

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List <int> triangles = new List<int>();
    [SerializeField] int sideCount = 4;
    [SerializeField] float radius = 4;
    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    public void GenerateVertex(Vector3 position, Vector3 previousPos)
    {
        int prevIndex = vertices.IndexOf(previousPos);
        vertices.Add(position);
        int ogIndex = vertices.IndexOf(position);

        for (int i = 0; i < sideCount; i++)
        {
            float angle = Mathf.PI * 2 * i / sideCount;
            Vector3 P = new Vector3((radius * Mathf.Sin(angle)) + position.x, position.y, (radius * Mathf.Cos(angle)) + position.z);
            vertices.Add(P);    
        }
        for (int i = 0; i < sideCount ; i++)
        {
            triangles.Add(ogIndex);
            triangles.Add(ogIndex + 1 + (i + 1) % sideCount);
            triangles.Add(ogIndex + 1 + i); 
        }
        if (prevIndex != -1)
        {
            CreateWalls(ogIndex, prevIndex);
        }
    }


    public void UpdateMesh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        Debug.Log(vertices.Count);

    }

    private void CreateWalls(int index, int prevIndex)
    {
        int size = sideCount;
        int bottomFirstIndex = prevIndex + 1;
        int topFirstIndex = index + 1;

        for(int i = 0;i < size;i++)
        {
            int indexer = 1;
            if(i == size - 1)
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
    //private void OnDrawGizmos()
    //{
    //    for(int i = 0; i < vertices.Count;i++)
    //    {
    //        Handles.Label(vertices[i], i.ToString());

    //    }
    //}

    public void ResetMesh()
    {
        vertices.Clear();
        triangles.Clear();

    }
}