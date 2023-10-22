using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KinectFrustumRenderer : MonoBehaviour
{
    [SerializeField] private float fov = 60;
    [SerializeField] private float farPlane = 5f;
    private float aspect = 1.2f;

    private void Start()
    {
        var frustumWidth = 2.0f * farPlane * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        var frustumHeight = frustumWidth / aspect;

        Vector3[] points = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(-frustumWidth / 2, frustumHeight / 2, farPlane),
            new Vector3(frustumWidth / 2, frustumHeight / 2, farPlane),
            new Vector3(frustumWidth / 2, -frustumHeight / 2, farPlane),
            new Vector3(-frustumWidth / 2, -frustumHeight / 2, farPlane)
        };

        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 11;
        lr.SetPositions(new Vector3[]
        {
            points[0],
            points[1],
            points[2],
            points[0],
            points[3],
            points[4],
            points[0],
            points[2],
            points[3],
            points[4],
            points[1]
        });
    }
}
