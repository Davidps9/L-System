using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicForceField : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 direction = ScreenToWorldRotator.MouseVelocity.normalized;
        Debug.Log(direction);
        if (other.gameObject.TryGetComponent<Branch>(out var branch))
        {
            branch.SetTargetRotationWithEase(Quaternion.LookRotation(direction), 1);
        }
    }
}
