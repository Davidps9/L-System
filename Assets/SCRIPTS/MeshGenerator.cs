using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]

public static class MeshGenerator
{
    public static Mesh GenerateMesh(Node[] nodes, int sideCount, string name = "Mesh")
    {
        MeshInfo meshInfo = new();
        meshInfo.sideCount = sideCount;

        for (int i = 0; i < nodes.Length; i++)
        {
            Vector3 angleBetween = nodes[i].rotation;
            Vector3 localAngleBetween = Vector3.zero;


            if (i + 1 < nodes.Length)
            {
                angleBetween = nodes[i].rotation + ((nodes[i + 1].rotation - nodes[i].rotation) / 2);
                localAngleBetween = (angleBetween - nodes[i].rotation);
            }

            Vector2 scale = new Vector2(
                Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.x * Mathf.Deg2Rad),
                Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad) == 0 ? float.MaxValue : 1 / Mathf.Cos(localAngleBetween.z * Mathf.Deg2Rad)
            );

            int vertexIndex = meshInfo.CreateVertex(nodes[i].position, angleBetween, scale, (i == 0 || i == nodes.Length - 1));

            if (nodes[i].parent != null)
            {
                int parentVertexIndex = meshInfo.vertices.IndexOf(nodes[i].parent.position);
                meshInfo.CreateWalls(vertexIndex, parentVertexIndex);
            }
        }

        return meshInfo.CreateMesh(name);
    }

    private static int CreateVertex(this MeshInfo meshInfo, Vector3 position, Vector3 rotation, Vector2 scale, bool addCap = false)
    {
        // Extract info from MeshInfo
        List<Vector3> vertices = meshInfo.vertices;
        List<int> triangles = meshInfo.triangles;
        //List<Vector2> uvs = meshInfo.uvs;
        int sideCount = meshInfo.sideCount;

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

    private static void CreateWalls(this MeshInfo meshInfo, int index, int prevIndex)
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

    private static Mesh CreateMesh(this MeshInfo meshInfo, string name)
    {
        Mesh mesh = new();
        mesh.name = name;
        mesh.SetVertices(meshInfo.vertices);
        mesh.SetTriangles(meshInfo.triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        Unwrapping.GenerateSecondaryUVSet(mesh);

        return mesh;
    }

    private class MeshInfo
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public int sideCount = 3;
    }
}