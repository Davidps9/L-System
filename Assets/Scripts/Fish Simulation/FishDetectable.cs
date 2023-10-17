using System;
using UnityEngine;

public abstract class FishDetectable : MonoBehaviour
{
    [NonSerialized] public Vector3 velocity = Vector3.zero;
    [NonSerialized] public bool affectsAlignment = false;
    [NonSerialized] public bool affectsSeparation = false;
    [NonSerialized] public bool affectsCohesion = false;

    public float Distance(FishDetectable otherFish)
    {
        return (transform.position - otherFish.transform.position).magnitude;
    }

    public float Distance(Collider collider)
    {
        return (transform.position - collider.bounds.ClosestPoint(transform.position)).magnitude;
    }
}