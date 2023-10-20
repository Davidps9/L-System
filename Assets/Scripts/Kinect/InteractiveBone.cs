using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class InteractiveBone : MonoBehaviour
{
    [Header("Fish repellant options")]
    [SerializeField] private float speedMultiplier = 2;
    [SerializeField] private float repellantWidth = 0.5f;
    [SerializeField] private float repellantDepth = 5;

    private BoxCollider repellantCollider;

    private void Awake()
    {
        repellantCollider = GetComponent<BoxCollider>();
    }

    public void Refresh(Vector3 firstPosition, Vector3 secondPosition)
    {
        Vector3 frontCenter = (firstPosition + secondPosition) * 0.5f;
        Vector3 thirdPosition = GetCameraPointAtDepth(secondPosition, repellantDepth);

        Vector3 center = (frontCenter + thirdPosition) * 0.5f;
        Vector3 size = new Vector3(repellantWidth, Vector3.Distance(firstPosition, secondPosition), Vector3.Distance(frontCenter, thirdPosition));

        transform.position = center;
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, thirdPosition - frontCenter);
        repellantCollider.size = size;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Fish>(out var fish))
        {
            fish.AvoidPoint(repellantCollider.ClosestPoint(other.transform.position), speedMultiplier);
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
