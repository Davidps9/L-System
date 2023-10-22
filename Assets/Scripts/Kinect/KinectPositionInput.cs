using UnityEngine;
using UnityEngine.SceneManagement;

public class KinectPositionInput : MonoBehaviour
{
    [SerializeField] private KinectPositioner kinectPositioner;

    [Header("Visualizers")]
    [SerializeField] private Transform kinectStandIn;
    [SerializeField] private Transform kinectMirroredFrustum;
    private Vector3 position = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    public void SetPositionX(string value)
    {
        if (value == "") { value = "0"; }
        if (float.TryParse(value, out float parsedValue))
        {
            position.x = parsedValue;
            UpdateObjects();
        }
    }

    public void SetPositionY(string value)
    {
        if (value == "") { value = "0"; }
        if (float.TryParse(value, out float parsedValue))
        {
            position.y = parsedValue;
            UpdateObjects();
        }
    }

    public void SetPositionZ(string value)
    {
        if (value == "") { value = "0"; }
        if (float.TryParse(value, out float parsedValue))
        {
            position.z = parsedValue;
            UpdateObjects();
        }
    }

    public void SetRotationX(string value)
    {
        if (value == "") { value = "0"; }
        if (float.TryParse(value, out float parsedValue))
        {
            rotation.x = parsedValue;
            UpdateObjects();
        }
    }

    public void SetRotationY(string value)
    {
        if (value == "") { value = "0"; }
        if (float.TryParse(value, out float parsedValue))
        {
            rotation.y = parsedValue;
            UpdateObjects();
        }
    }

    public void SetRotationZ(string value)
    {
        if (value == "") { value = "0"; }
        if (float.TryParse(value, out float parsedValue))
        {
            rotation.z = parsedValue;
            UpdateObjects();
        }
    }

    public void UpdateObjects()
    {
        kinectPositioner.SetPosition(position);
        kinectPositioner.SetRotation(rotation);
        kinectStandIn.SetPositionAndRotation(position, Quaternion.Euler(rotation));

        // mirroredFrustum
        Vector3 mirroredPosition = new Vector3(position.x, position.y, -position.z);
        Quaternion mirroredRotation = Quaternion.Euler(rotation.x, -rotation.y + 180, rotation.z);
        kinectMirroredFrustum.SetPositionAndRotation(mirroredPosition, mirroredRotation);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("FinalScene");
    }
}
