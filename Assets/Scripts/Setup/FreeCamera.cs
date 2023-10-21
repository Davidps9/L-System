using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamera : MonoBehaviour
{
    private Vector2 look = Vector2.zero;
    private Vector2 move = Vector2.zero;
    private bool movementEnabled = true;
    [SerializeField]
    private float speed = 2f;
    private Vector2 rotation = Vector2.zero;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnToggleCamera()
    {
        movementEnabled = !movementEnabled;
        Cursor.lockState = movementEnabled ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementEnabled)
        {
            return;
        }

        rotation += look;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
        transform.localRotation = Quaternion.Euler(-rotation.y, rotation.x, 0);
        transform.Translate(new Vector3(speed * Time.deltaTime * move.x, 0, speed * Time.deltaTime * move.y), Space.Self);
    }
}