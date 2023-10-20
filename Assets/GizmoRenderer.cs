using UnityEngine;

public class GizmoRenderer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(0, 1, 4), new Vector3(4, 2, 8));
    }
}
