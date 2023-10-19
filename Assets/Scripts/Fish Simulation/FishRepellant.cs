using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FishRepellant : MonoBehaviour
{
    [SerializeField] private Collider repellantCollider;
    [SerializeField] private float speedMultiplier = 2;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Fish>(out var fish))
        {
            fish.AvoidPoint(repellantCollider.ClosestPoint(other.transform.position), speedMultiplier);
        }
    }
}
