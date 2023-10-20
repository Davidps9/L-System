using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class InteractiveBody : MonoBehaviour
{
    //private static readonly Dictionary<Kinect.JointType, Kinect.JointType> boneMap = new()
    //{
    //    { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
    //    { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
    //    { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
    //    { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

    //    { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
    //    { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
    //    { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
    //    { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

    //    { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
    //    { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
    //    { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
    //    { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
    //    { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
    //    { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

    //    { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
    //    { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
    //    { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
    //    { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
    //    { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
    //    { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

    //    { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
    //    { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
    //    { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
    //    { Kinect.JointType.Neck, Kinect.JointType.Head },
    //};
    private static readonly Dictionary<Kinect.JointType, Kinect.JointType> boneMap = new()
    {
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },

        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },

        { Kinect.JointType.HandLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },

        { Kinect.JointType.HandRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineShoulder },
    };

    private Transform kinectPosition;
    private Dictionary<Kinect.JointType, InteractiveBone> bones = new();

    public void Init(GameObject bonePrefab, Transform kinectPosition)
    {
        this.kinectPosition = kinectPosition;

        foreach (Kinect.JointType jt in boneMap.Keys)
        {
            GameObject jointObj = Instantiate(bonePrefab, transform);
            jointObj.name = jt.ToString();
            if (jointObj.TryGetComponent<InteractiveBone>(out var bone))
            {
                bones.Add(jt, bone);
            }
            else
            {
                Debug.LogError("InteractiveBone script not found in prefab");
            }
        }

        Debug.Log("fwd: " + kinectPosition.forward + ", right: " + kinectPosition.right);
    }

    public void Refresh(Kinect.Body body)
    {
        foreach (KeyValuePair<Kinect.JointType, Kinect.JointType> bone in boneMap)
        {
            Vector3 firstPosition = GetVector3FromJoint(body.Joints[bone.Key]);
            Vector3 secondPosition = GetVector3FromJoint(body.Joints[bone.Value]);

            if (bones.TryGetValue(bone.Key, out var interactiveBone))
            {
                interactiveBone.Refresh(firstPosition, secondPosition);
            }
            else
            {
                Debug.LogError("InteractiveBone not found for " + bone.Key);
            }
        }

        // OLD
        //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        //{
        //    Kinect.Joint sourceJoint = body.Joints[jt];
        //    Kinect.Joint? targetJoint = null;

        //    if (boneMap.ContainsKey(jt))
        //    {
        //        targetJoint = body.Joints[boneMap[jt]];
        //    }

        //    Transform jointObj = transform.Find(jt.ToString());
        //    jointObj.localPosition = GetVector3FromJoint(sourceJoint);

        //    LineRenderer lr = jointObj.GetComponent<LineRenderer>();
        //    if (targetJoint.HasValue)
        //    {
        //        lr.SetPosition(0, jointObj.localPosition);
        //        lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
        //        lr.startColor = GetColorForState(sourceJoint.TrackingState);
        //        lr.endColor = GetColorForState(targetJoint.Value.TrackingState);
        //    }
        //    else
        //    {
        //        lr.enabled = false;
        //    }
        //}
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        Vector3 worldPos = kinectPosition.TransformPoint(-joint.Position.X, joint.Position.Y, joint.Position.Z);
        return new Vector3(worldPos.x, worldPos.y, -worldPos.z);
    }
}
