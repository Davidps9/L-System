using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class BodyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject bonePrefab;
    private Dictionary<ulong, InteractiveBody> bodies = new Dictionary<ulong, InteractiveBody>();
    private KinectDataManager dataManager;

    void Update()
    {
        dataManager = KinectDataManager.instance;
        if (dataManager == null)
        {
            return;
        }

        Kinect.Body[] data = dataManager.GetData();
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

        List<ulong> knownIds = new List<ulong>(bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(bodies[trackingId].gameObject);
                bodies.Remove(trackingId);
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
                if (!bodies.ContainsKey(body.TrackingId))
                {
                    bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                bodies[body.TrackingId].Refresh(body);
            }
        }
    }

    private InteractiveBody CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        body.transform.parent = transform;
        InteractiveBody ib = body.AddComponent<InteractiveBody>();
        ib.Init(bonePrefab, dataManager.kinectPosition);
        return ib;
    }
}
