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
        Vector3 force = ScreenToWorldRotator.MouseVelocity * 10;
        Debug.Log(force);
        other.attachedRigidbody.AddForce(force);
    }
}
