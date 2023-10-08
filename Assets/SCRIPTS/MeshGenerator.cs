using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]

public static class MeshGenerator
{
    public static Mesh GenerateMesh(Node[] nodes, int sideCount)
    {
        MeshInfo meshInfo = new();
        meshInfo.sideCount = sideCount;

        for (int i = 0; i < nodes.Length; i++)
        {
            Vector3 angleBetween = Vector3.zero;
            Vector3 localAngleBetween = Vector3.zero;


            if (i + 1 < nodes.Length)
            {
                angleBetween = nodes[i].rotation + ((nodes[i + 1].rotation - nodes[i].rotation) / 2);
                localAngleBetween = (angleBetween - nodes[i].rotation);
                Debug.Log("parent: " + nodes[i].rotation + ", node: " + nodes[i].rotation + ", angle: " + angleBetween + ", local angle: " + localAngleBetween);
            }

            Vector2 scale = new Vector2(
                Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad),
                Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad)
            );

            int nodeIndex = meshInfo.GenerateVertex(nodes[i].position, angleBetween, scale, (i == 0 || i == nodes.Length - 1));

            if (nodes[i].parent != null)
            {
                int parentIndex = meshInfo.vertices.IndexOf(nodes[i].parent.position);
                CreateWalls(ref meshInfo, nodeIndex, parentIndex);
            }
        }

        Mesh mesh = new();
        mesh.SetVertices(meshInfo.vertices);
        mesh.SetTriangles(meshInfo.triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        Unwrapping.GenerateSecondaryUVSet(mesh);

        //Debug.Log(mesh.vertices.Length);
        return mesh;
    }

    private static int GenerateVertex(this MeshInfo meshInfo, Vector3 position, Vector3 rotation, Vector2 scale, bool addCap = false)
    {
        // Extract info from MeshInfo
        List<Vector3> vertices = meshInfo.vertices;
        List<int> triangles = meshInfo.triangles;
        //List<Vector2> uvs = meshInfo.uvs;
        int sideCount = meshInfo.sideCount;

        //Debug.Log("local rotation: " + node.localRotation + ", scale: " + scale);

        vertices.Add(position);
        //uvs.Add(new Vector2(node.position.x, node.position.z) * node.position.y);

        Quaternion rotationQuaternion = Quaternion.Euler(rotation.x, 0, rotation.z);
        for (int i = 0; i < sideCount; i++)
        {
            float baseAngle = Mathf.PI * 2 * i / sideCount;

            Vector3 vertex = new Vector3(Mathf.Sin(baseAngle) * scale.y, 0, Mathf.Cos(baseAngle) * scale.x);
            vertex = rotationQuaternion * vertex;
            vertex += position;
            vertices.Add(vertex);
            //uvs.Add(new Vector2(vertex.x, vertex.z) * vertex.y);
        }

        int nodeIndex = vertices.IndexOf(position);
        if (addCap)
        {
            for (int i = 0; i < sideCount; i++)
            {
                triangles.Add(nodeIndex);
                triangles.Add(nodeIndex + 1 + (i + 1) % sideCount);
                triangles.Add(nodeIndex + 1 + i);
            }
        }

        return nodeIndex;
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