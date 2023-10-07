using System.Collections.Generic;
using UnityEngine;

public class MeshInfo
{
    public List<Vector2> uvs = new List<Vector2>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();

    public int sideCount = 3;
    public float radius = 1;

    public void Reset()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }
}
