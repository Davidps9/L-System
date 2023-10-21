using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FrustumGizmo : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float nearPlane = 0.1f;
    [SerializeField] private float farPlane = 100f;
    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        var frustumHeight = 2.0f * nearPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * cam.aspect;
        Debug.Log(cam.name + " frustum size at " + nearPlane + "m -> " + frustumWidth + "m x " + frustumHeight + "m");
    }

    private void OnDrawGizmos()
    {
        if (cam)
        {
            Gizmos.color = color;
            Gizmos.matrix = cam.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero,
                cam.fieldOfView,
                farPlane,
                nearPlane,
                cam.aspect);
        }
    }
}