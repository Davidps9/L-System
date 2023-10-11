using UnityEngine;

public class ScreenToWorldRotator : MonoBehaviour
{
    private void Start()
    {
        transform.position = Camera.main.transform.position;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(ray.direction);
    }
}
