using UnityEditor;
using UnityEngine;

public class KinectPositioner : MonoBehaviour
{
    [SerializeField] private float fov = 60;
    [SerializeField] private float nearPlane = 0.2f;
    [SerializeField] private float farPlane = 5f;

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void OnDrawGizmos()
    {
        // Kinect position
        Gizmos.color = new Color(0, 1, 0, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.2f);

        if (Selection.Contains(gameObject)) { return; }

        // kinect mirrored frustum
        Gizmos.color = Color.green;
        Vector3 mirroredPosition = new Vector3(transform.position.x, transform.position.y, -1 * transform.position.z);
        Quaternion mirroredRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
        Gizmos.matrix = Matrix4x4.TRS(mirroredPosition, mirroredRotation, transform.localScale);
        Gizmos.DrawFrustum(Vector3.zero, fov, farPlane, nearPlane, 1.2f);
    }

    private void OnDrawGizmosSelected()
    {
        // kinect real frustum
        Gizmos.color = new Color(0, 1, 0, 0.4f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, fov, farPlane, nearPlane, 1.2f);
    }
}
