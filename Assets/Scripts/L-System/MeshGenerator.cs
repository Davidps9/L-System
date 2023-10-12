using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

            int vertexIndex = meshInfo.CreateVertex(nodes[i].position, angleBetween, scale);


            meshInfo.CreateUVs(vertexIndex, (i + 1) / nodes.Length);

            if (i == 0 || i == nodes.Length - 1)
            {
                meshInfo.CreateCap(vertexIndex);
            }

            if (nodes[i].parent != null)
            {
                int parentVertexIndex = vertexIndex - (sideCount + 1);
                meshInfo.CreateWalls(vertexIndex, parentVertexIndex);
            }
        }

        return meshInfo.CreateMesh(name);
    }

    private static int CreateVertex(this MeshInfo meshInfo, Vector3 position, Vector3 rotation, Vector2 scale)
    {
        // Extract info from MeshInfo
        List<Vector3> vertices = meshInfo.vertices;
        int sideCount = meshInfo.sideCount;

        vertices.Add(position);
        int vertexIndex = vertices.Count - 1;

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

        return vertexIndex;
    }

    private static void CreateCap(this MeshInfo meshInfo, int index)
    {
        for (int x = 0; x < meshInfo.sideCount; x++)
        {
            meshInfo.triangles.Add(index);
            meshInfo.triangles.Add(index + 1 + (x + 1) % meshInfo.sideCount);
            meshInfo.triangles.Add(index + 1 + x);
        }
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

    private static void CreateUVs(this MeshInfo meshInfo, int index, float y)
    {
        if (meshInfo.uvs.Count != index)
        {
            Debug.LogWarning("UV list has wrong number of vertices. Aborting creation.");
            return;
        }

        int sizeX = 1 / meshInfo.sideCount;
        meshInfo.uvs.Add(new Vector2(0.5f, y));

        for (int i = 0; i < meshInfo.sideCount; i++)
        {
            meshInfo.uvs.Add(new Vector2(sizeX * i, y));
        }
    }

    private static Mesh CreateMesh(this MeshInfo meshInfo, string name)
    {
        Mesh mesh = new();
        mesh.name = name;
        mesh.SetVertices(meshInfo.vertices);
        mesh.SetTriangles(meshInfo.triangles, 0);

        if (meshInfo.uvs.Count == mesh.vertexCount)
        {
            mesh.SetUVs(1, meshInfo.uvs);
        }
        else
        {
            Unwrapping.GenerateSecondaryUVSet(mesh);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();



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