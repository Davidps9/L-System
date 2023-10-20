using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class Fish : FishDetectable
{
    private FishSimulation simulation;
    private List<FishDetectable> fishInRange = new();

    public void Init(FishSimulation simulation)
    {
        affectsAlignment = true;
        affectsSeparation = true;
        affectsCohesion = true;

        this.simulation = simulation;
        velocity = Random.insideUnitSphere * simulation.maxSpeed;
        GetComponent<SphereCollider>().radius = simulation.visualRange / transform.localScale.y;
    }

    private void Update()
    {
        FlyTowardsCenter();
        AvoidOthers();
        MatchVelocity();
        LimitSpeed();
        KeepWithinBounds();
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    #region Cohesion
    // Find the center of mass of the other boids and adjust velocity slightly to point towards the center of mass.
    private void FlyTowardsCenter()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (FishDetectable fish in fishInRange)
        {
            if (!fish.affectsCohesion) { continue; }
            center += fish.transform.position;
            count++;
        }
        if (count > 0)
        {
            center /= count;
            velocity += (center - transform.position).normalized * simulation.centeringFactor;
        }
    }
    #endregion

    #region Separation
    // Move away from other boids that are too close to avoid colliding
    private void AvoidOthers()
    {
        foreach (FishDetectable fish in fishInRange)
        {
            if (!fish.affectsSeparation) { continue; }
            if (fish.Distance(this) > simulation.minDistance) { return; }
            velocity += (transform.position - fish.transform.position).normalized * simulation.avoidFactor;
        }
    }

    public void AvoidPoint(Vector3 point, float speedMultiplier)
    {
        velocity += (transform.position - point).normalized * speedMultiplier;
    }

    #endregion

    #region Alignment
    // Find the average velocity (speed and direction) of the other boids and adjust velocity slightly to match.
    private void MatchVelocity()
    {
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;

        foreach (FishDetectable fish in fishInRange)
        {
            if (!fish.affectsAlignment) { continue; }
            avgVelocity += fish.velocity;
            count++;
        }
        if (count > 0)
        {
            avgVelocity /= count;
            velocity += (avgVelocity - velocity).normalized * simulation.matchingFactor;
        }
    }
    #endregion

    #region Other

    private void LimitSpeed()
    {
        float speed = velocity.magnitude;
        if (speed > simulation.maxSpeed)
        {
            velocity = (velocity / speed) * simulation.maxSpeed;
        }
    }

    private void KeepWithinBounds()
    {
        if (transform.position.x > simulation.transform.position.x + simulation.bounds.x)
        {
            velocity.x -= simulation.turnFactor;
        }
        if (transform.position.x < simulation.transform.position.x - simulation.bounds.x)
        {
            velocity.x += simulation.turnFactor;
        }
        if (transform.position.y > simulation.transform.position.y + simulation.bounds.y)
        {
            velocity.y -= simulation.turnFactor;
        }
        if (transform.position.y < simulation.transform.position.y - simulation.bounds.y)
        {
            velocity.y += simulation.turnFactor;
        }
        if (transform.position.z > simulation.transform.position.z + simulation.bounds.z)
        {
            velocity.z -= simulation.turnFactor;
        }
        if (transform.position.z < simulation.transform.position.z - simulation.bounds.z)
        {
            velocity.z += simulation.turnFactor;
        }
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Fish fish))
        {
            if (fishInRange.Contains(fish)) { return; }
            fishInRange.Add(fish);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Fish fish))
        {
            if (!fishInRange.Contains(fish)) { return; }
            fishInRange.Remove(fish);
        }
    }
}
