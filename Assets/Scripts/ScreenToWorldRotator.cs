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

    public static Vector2 MouseVelocity { get => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
}
