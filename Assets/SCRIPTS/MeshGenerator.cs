using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]

public static class MeshGenerator
{
    public static void GenerateMesh(this MeshInfo meshInfo, Node[] nodes)
    {
        foreach (Node node in nodes)
        {
            GenerateVertex(ref meshInfo, node, false);
        }
    }
    public static void GenerateMesh(this Branch branch)
    {
        foreach (Node node in branch.nodes)
        {
            GenerateVertex(ref branch.meshInfo, node, false);
        }
    }

    private static void GenerateVertex(ref MeshInfo meshInfo, Node node, bool addCap)
    {
        // Extract info from MeshInfo
        List<Vector3> vertices = meshInfo.vertices;
        List<int> triangles = meshInfo.triangles;
        //List<Vector2> uvs = meshInfo.uvs;
        int sideCount = meshInfo.sideCount;
        float radius = meshInfo.radius;

        Vector3 angleBetween = Vector3.zero;
        Vector3 localAngleBetween = Vector3.zero;
        if (node.parent != null)
        {
            angleBetween = node.parent.rotation + ((node.rotation - node.parent.rotation) / 2);
            localAngleBetween = (angleBetween - node.parent.rotation);
            Debug.Log("parent: " + node.parent.rotation + ", node: " + node.rotation + ", angle: " + angleBetween + ", local angle: " + localAngleBetween);
        }

        Vector2 scale = new Vector2(
            Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad),
            Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad)
        );

        //Debug.Log("local rotation: " + node.localRotation + ", scale: " + scale);

        vertices.Add(node.position);
        //uvs.Add(new Vector2(node.position.x, node.position.z) * node.position.y);

        int nodeIndex = vertices.IndexOf(node.position);

        Quaternion rotation = Quaternion.Euler(angleBetween.x, 0, angleBetween.z);
        for (int i = 0; i < sideCount; i++)
        {
            float baseAngle = Mathf.PI * 2 * i / sideCount;

            Vector3 vertex = new Vector3(radius * Mathf.Sin(baseAngle) * scale.y, 0, radius * Mathf.Cos(baseAngle) * scale.x);
            vertex = rotation * vertex;
            vertex += node.position;
            vertices.Add(vertex);
            //uvs.Add(new Vector2(vertex.x, vertex.z) * vertex.y);
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
            CreateWalls(ref meshInfo, nodeIndex, parentIndex);
        }
    }

    //public void UpdateMesh()
    //{
    //    mesh.SetVertices(vertices);
    //    mesh.SetTriangles(triangles, 0);
    //    mesh.RecalculateBounds();
    //    mesh.RecalculateTangents();
    //    mesh.RecalculateNormals();
    //    Unwrapping.GenerateSecondaryUVSet(mesh);
    //    //Debug.Log(vertices.Count);
    //}

    private static void CreateWalls(ref MeshInfo meshInfo, int index, int prevIndex)
    {
        int size = meshInfo.sideCount;
        List<int> triangles = meshInfo.triangles;

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

    public static T Last<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }

    public static T First<T>(this List<T> list)
    {
        return list[0];
    }
}