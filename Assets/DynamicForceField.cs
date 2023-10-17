using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicForceField : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Vector3 direction = ScreenToWorldRotator.MouseVelocity.normalized;
        Debug.Log(direction);
        if (other.gameObject.TryGetComponent<Branch>(out var branch))
        {
            branch.ApplyForce(ScreenToWorldRotator.MouseVelocity);
        }
    }
}
