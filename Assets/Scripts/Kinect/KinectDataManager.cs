using UnityEngine;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class KinectDataManager : MonoBehaviour
{
    public Transform kinectPosition;
    private KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;

    public static KinectDataManager instance;

    public Body[] GetData()
    {
        return _Data;
    }


    void Start()
    {
        // initialize singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // setup sensor

        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }

    void Update()
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(_Data);

                frame.Dispose();
                frame = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }

    public static Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    public Vector3 GetMirroredVector3FromJoint(Joint joint)
    {
        Vector3 worldPos = kinectPosition.TransformPoint(-joint.Position.X, joint.Position.Y, joint.Position.Z);
        return new Vector3(worldPos.x, worldPos.y, -worldPos.z);
    }
}
