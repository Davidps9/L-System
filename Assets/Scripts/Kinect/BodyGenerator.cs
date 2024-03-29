﻿using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

public class BodyGenerator : MonoBehaviour
{
    [SerializeField] private GameObject bonePrefab;
    [SerializeField] private GameObject handIndicatorPrefab;

    [Header("L-System Generation")]
    [SerializeField] private float velocityThreshold = 1f;
    [SerializeField] private float stillTime = 2f;

    private Dictionary<ulong, InteractiveBody> bodies = new Dictionary<ulong, InteractiveBody>();
    private KinectDataManager dataManager;

    Kinect.Body[] data;
    List<ulong> trackedIds = new List<ulong>();
    List<ulong> knownIds = new List<ulong>();

    void Update()
    {
        dataManager = KinectDataManager.instance;
        if (dataManager == null)
        {
            return;
        }

        data = dataManager.GetData();
        if (data == null)
        {
            return;
        }

        trackedIds.Clear();
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

        knownIds = new List<ulong>(bodies.Keys);

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
        ib.Init(id, bonePrefab, handIndicatorPrefab, velocityThreshold, stillTime);
        return ib;
    }
}
