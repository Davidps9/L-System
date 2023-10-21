using UnityEditor;
using UnityEngine;

public class InteractiveBone : MonoBehaviour
{
    [Header("Fish repellant options")]
    [SerializeField] private float speedMultiplier = 0.1f;
    [SerializeField] private float repellantDepth = 5;

    private Vector3 boneCenter = Vector3.zero;
    private Vector3 prevBoneCenter = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private Collider repellantCollider;
    private LineRenderer lineRenderer;

    public void Init(bool ignoreDepth)
    {
        if (ignoreDepth)
        {
            repellantCollider = gameObject.AddComponent<SphereCollider>();
        }
        else
        {
            repellantCollider = gameObject.AddComponent<CapsuleCollider>();
        }
        repellantCollider.isTrigger = true;

        // DEBUG LINE
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
    }

    public void Refresh(Vector3 firstPosition, Vector3 secondPosition)
    {
        // debug line
        lineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });

        prevBoneCenter = boneCenter;
        boneCenter = (firstPosition + secondPosition) * 0.5f;
        velocity = (boneCenter - prevBoneCenter) / Time.deltaTime;

        float radius = Vector3.Distance(firstPosition, secondPosition) / 2;

        if (repellantCollider is CapsuleCollider)
        {
            CapsuleCollider capsuleCollider = repellantCollider as CapsuleCollider;

            Vector3 thirdPosition = GetCameraPointAtDepth(boneCenter, repellantDepth);
            Vector3 colliderCenter = (boneCenter + thirdPosition) * 0.5f;
            float height = Vector3.Distance(boneCenter, thirdPosition);

            capsuleCollider.radius = radius;
            capsuleCollider.height = height;
            transform.position = colliderCenter;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, thirdPosition - boneCenter);
        }
        else if (repellantCollider is SphereCollider)
        {
            SphereCollider sphereCollider = repellantCollider as SphereCollider;
            sphereCollider.radius = radius;
            transform.position = boneCenter;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Branch>(out var branch))
        {
            branch.ApplyForce(velocity);
        }
        else if (other.gameObject.TryGetComponent<Fish>(out var fish))
        {
            fish.AvoidPoint(repellantCollider.ClosestPoint(other.transform.position), velocity.magnitude * speedMultiplier);
        }
    }

    public static Vector3 GetCameraPointAtDepth(Vector3 originalPoint, float depth)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(originalPoint);
        screenPoint.z += depth;
        Vector3 newPoint = Camera.main.ScreenToWorldPoint(screenPoint);

        return newPoint;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(boneCenter, velocity.ToString());
    }
}
