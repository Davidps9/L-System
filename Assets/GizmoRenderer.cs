using UnityEngine;

public class GizmoRenderer : MonoBehaviour
{
    [SerializeField] private Color color = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(new Vector3(0, 1, 4), new Vector3(4, 2, 8));
    }
}
