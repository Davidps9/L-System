using System;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class InteractiveBody : MonoBehaviour
{
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

    [NonSerialized] public ulong id;
    private Dictionary<Kinect.JointType, InteractiveBone> bones = new();
    private Vector2 lean = Vector2.zero;

    private float velocityThreshold = 1f;
    private float stillTime = 2f;

    private HandIndicator leftHandIndicator, rightHandIndicator;

    private Vector3 leftHandPos = Vector3.zero, leftHandPrevPos = Vector3.zero;
    private Vector3 leftHandVelocity = Vector3.zero;
    private Kinect.HandState leftHandState = Kinect.HandState.Unknown;

    private Vector3 rightHandPos = Vector3.zero, rightHandPrevPos = Vector3.zero;
    private Vector3 rightHandVelocity = Vector3.zero;
    private Kinect.HandState rightHandState = Kinect.HandState.Unknown;

    public void Init(ulong id, GameObject bonePrefab, GameObject handIndicatorPrefab, float velocityThreshold, float stillTime)
    {
        this.id = id;
        this.velocityThreshold = velocityThreshold;
        this.stillTime = stillTime;

        foreach (Kinect.JointType jt in boneMap.Keys)
        {
            GameObject jointObj = Instantiate(bonePrefab, transform);
            jointObj.name = jt.ToString();
            if (jointObj.TryGetComponent<InteractiveBone>(out var bone))
            {
                bone.Init(jt == Kinect.JointType.SpineBase);
                bones.Add(jt, bone);
            }
            else
            {
                Debug.LogError("InteractiveBone script not found in prefab");
            }
        }

        // spawn hand indicators
        leftHandIndicator = Instantiate(handIndicatorPrefab, transform).GetComponent<HandIndicator>();
        leftHandIndicator.SetSide(true);
        rightHandIndicator = Instantiate(handIndicatorPrefab, transform).GetComponent<HandIndicator>();
        rightHandIndicator.SetSide(false);
    }

    public void Refresh(Kinect.Body body)
    {
        // hands
        leftHandState = body.HandLeftState;
        rightHandState = body.HandRightState;

        leftHandPrevPos = leftHandPos;
        leftHandPos = KinectDataManager.instance.GetMirroredVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        if (leftHandPos != leftHandPrevPos)
        {
            leftHandVelocity = (leftHandPos - leftHandPrevPos) / Time.deltaTime;
        }

        rightHandPrevPos = rightHandPos;
        rightHandPos = KinectDataManager.instance.GetMirroredVector3FromJoint(body.Joints[Kinect.JointType.HandRight]);
        if (rightHandPos != rightHandPrevPos)
        {
            rightHandVelocity = (rightHandPos - rightHandPrevPos) / Time.deltaTime;
        }

        UpdateHandIndicators();
        TryGenerateLSystem();

        // lean
        if (body.LeanTrackingState > 0)
        {
            lean = new Vector2(body.Lean.X, body.Lean.Y);
        }

        // Create bones
        foreach (KeyValuePair<Kinect.JointType, Kinect.JointType> bone in boneMap)
        {
            if (body.Joints[bone.Key].TrackingState == 0 || body.Joints[bone.Value].TrackingState == 0)
            {
                continue;
            }

            Vector3 firstPosition = KinectDataManager.instance.GetMirroredVector3FromJoint(body.Joints[bone.Key]);
            Vector3 secondPosition = KinectDataManager.instance.GetMirroredVector3FromJoint(body.Joints[bone.Value]);

            if (bones.TryGetValue(bone.Key, out var interactiveBone))
            {
                interactiveBone.Refresh(firstPosition, secondPosition);
            }
            else
            {
                Debug.LogError("InteractiveBone not found for " + bone.Key);
            }
        }
    }

    private void UpdateHandIndicators()
    {
        if (leftHandState != Kinect.HandState.Unknown)
        {
            leftHandIndicator.SnapTo(leftHandPos);
        }

        if (rightHandState != Kinect.HandState.Unknown)
        {
            rightHandIndicator.SnapTo(rightHandPos);
        }
    }

    private float timer = 0;
    private void TryGenerateLSystem()
    {
        float adjustedThreshold = velocityThreshold * ((timer / stillTime) + 1);
        if (leftHandVelocity.magnitude < adjustedThreshold && rightHandVelocity.magnitude < adjustedThreshold)
        {
            if (timer < stillTime)
            {
                if (leftHandState == Kinect.HandState.Open && rightHandState == Kinect.HandState.Open)
                {
                    timer += Time.deltaTime;

                    leftHandIndicator.SetFill(timer / stillTime);
                    rightHandIndicator.SetFill(timer / stillTime);
                    return;
                }
            }
            else
            {
                if (leftHandState == Kinect.HandState.Closed && rightHandState == Kinect.HandState.Closed)
                {
                    timer = 0;
                    LSystemGenerator.instance.Generate(bones[Kinect.JointType.SpineBase].transform.position, lean, "L-System:" + id);
                }
                return;
            }
        }

        timer = 0;

        leftHandIndicator.SetFill(0);
        rightHandIndicator.SetFill(0);
    }
}
