using System.Collections.Generic;
using UnityEngine;

public class MeshInfo
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();
    public int sideCount = 3;

    public void Reset()
    {
        vertices.Clear();
        triangles.Clear();
        normals.Clear();
        uvs.Clear();
    }
}
