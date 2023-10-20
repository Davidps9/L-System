using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(KinectDataManager))]
public class BodyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject bonePrefab;
    [SerializeField] private Transform kinectPosition;
    private Dictionary<ulong, InteractiveBody> _Bodies = new Dictionary<ulong, InteractiveBody>();
    private KinectDataManager _BodyManager;

    void Update()
    {
        _BodyManager = GetComponent<KinectDataManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId].gameObject);
                _Bodies.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                _Bodies[body.TrackingId].Refresh(body);
            }
        }
    }

    private InteractiveBody CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        body.transform.parent = transform;
        InteractiveBody ib = body.AddComponent<InteractiveBody>();
        ib.Init(bonePrefab, kinectPosition);
        return ib;
    }
}
