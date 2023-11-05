using UnityEngine;

public class InteractiveBoneCollider : MonoBehaviour
{
    [Header("Fish repellant options")]
    [SerializeField] private float speedMultiplier = 0.1f;
    [SerializeField] private float repellantDepth = 5;
    private Collider repellantCollider;
    private Vector2 velocity;

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
    }

    public void Refresh(float boneLength, Vector3 boneCenter, Vector2 boneVelocity)
    {
        velocity = boneVelocity;

        if (repellantCollider is CapsuleCollider)
        {
            CapsuleCollider capsuleCollider = repellantCollider as CapsuleCollider;

            Vector3 thirdPosition = GetCameraPointAtDepth(boneCenter, repellantDepth);
            Vector3 colliderCenter = (boneCenter + thirdPosition) * 0.5f;
            float height = Vector3.Distance(boneCenter, thirdPosition);

            capsuleCollider.radius = boneLength / 2;
            capsuleCollider.height = height;
            transform.position = colliderCenter;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, thirdPosition - boneCenter);
        }
        else if (repellantCollider is SphereCollider)
        {
            SphereCollider sphereCollider = repellantCollider as SphereCollider;
            sphereCollider.radius = boneLength / 2;
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
}
