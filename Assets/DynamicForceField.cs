using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DynamicForceField : MonoBehaviour
{
    //[SerializeField] private float delay = 0.3f;
    //private List<Vector2> velocities = new() { Vector2.zero };
    //private bool readInput = false;

    //private void Start()
    //{
    //    StartCoroutine(AwaitDelay());
    //}

    //private IEnumerator AwaitDelay()
    //{
    //    yield return new WaitForSeconds(delay);
    //    readInput = true;
    //}

    //private void LateUpdate()
    //{
    //    velocities.Add(ScreenToWorldRotator.MouseVelocity);
    //    if (readInput)
    //    {
    //        velocities.RemoveAt(0);
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Branch>(out var branch))
        {
            //branch.ApplyForce(velocities[0]);
            branch.ApplyForce(ScreenToWorldRotator.MouseVelocity);
        }
    }
}
