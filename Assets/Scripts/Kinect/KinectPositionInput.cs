using UnityEngine;
using UnityEngine.SceneManagement;

public class KinectPositionInput : MonoBehaviour
{
    [SerializeField] private KinectPositioner kinectPositioner;
    private Vector3 position = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    public void SetPositionX(string value)
    {
        if (value == "") { return; }
        float x = float.Parse(value);
        position.x = x;
        kinectPositioner.SetPosition(position);
    }

    public void SetPositionY(string value)
    {
        if (value == "") { return; }
        float y = float.Parse(value);
        position.y = y;
        kinectPositioner.SetPosition(position);
    }

    public void SetPositionZ(string value)
    {
        if (value == "") { return; }
        float z = float.Parse(value);
        position.z = z;
        kinectPositioner.SetPosition(position);
    }

    public void SetRotationX(string value)
    {
        if (value == "") { return; }
        float x = float.Parse(value);
        rotation.x = x;
        kinectPositioner.SetRotation(rotation);
    }

    public void SetRotationY(string value)
    {
        if (value == "") { return; }
        float y = float.Parse(value);
        rotation.y = y;
        kinectPositioner.SetRotation(rotation);
    }

    public void SetRotationZ(string value)
    {
        if (value == "") { return; }
        float z = float.Parse(value);
        rotation.z = z;
        kinectPositioner.SetRotation(rotation);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("FinalScene");
    }
}
