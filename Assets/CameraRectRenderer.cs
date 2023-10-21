using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CameraRectRenderer : MonoBehaviour
{
    [SerializeField] private float nearPlane;
    [SerializeField] private float fov;

    void Start()
    {
        var aspect = Screen.width / (float)Screen.height;
        var frustumWidth = 2.0f * nearPlane * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        var frustumHeight = frustumWidth / aspect;

        Debug.Log(fov + " " + aspect);

        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 4;
        lr.SetPositions(new Vector3[]
        {
            new Vector3(-frustumWidth / 2, 0, 0),
            new Vector3(-frustumWidth / 2, frustumHeight, 0),
            new Vector3(frustumWidth / 2, frustumHeight, 0),
            new Vector3(frustumWidth / 2, 0, 0),
        });
    }
}
