using UnityEngine;

public class GizmoRenderer : MonoBehaviour
{
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Vector3 size = Vector3.one;
    [SerializeField] private Color color = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }
}
