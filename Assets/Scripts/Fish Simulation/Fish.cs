using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Fish : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private FishSimulation simulation;
    private Collider Collider => GetComponent<Collider>();

    public void Init(FishSimulation simulation)
    {
        this.simulation = simulation;
        velocity = Random.insideUnitSphere * simulation.maxSpeed;
    }

    private void Update()
    {
        FlyTowardsCenter();
        AvoidOthers();
        MatchVelocity();
        LimitSpeed();
        KeepWithinBounds();
        Debug.Log(velocity);
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    // Coherence
    // Find the center of mass of the other boids and adjust velocity slightly to point towards the center of mass.
    private void FlyTowardsCenter()
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        Fish[] fishInRange = GetFishInRange(simulation.visualRange);
        foreach (Fish fish in fishInRange)
        {
            center += fish.transform.position;
            count++;
        }
        if (count > 0)
        {
            center /= count;
            velocity += (center - transform.position).normalized * simulation.centeringFactor;
        }
    }

    // Avoidance
    // Move away from other boids that are too close to avoid colliding
    private void AvoidOthers()
    {
        Fish[] fishInRange = GetFishInRange(simulation.minDistance);
        foreach (Fish fish in fishInRange)
        {
            velocity += (transform.position - fish.transform.position).normalized * simulation.avoidFactor;
        }
    }

    // Cohesion
    // Find the average velocity (speed and direction) of the other boids and adjust velocity slightly to match.
    private void MatchVelocity()
    {
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;

        Fish[] fishInRange = GetFishInRange(simulation.visualRange);
        foreach (Fish fish in fishInRange)
        {
            avgVelocity += fish.velocity;
            count++;
        }
        if (count > 0)
        {
            avgVelocity /= count;
            velocity += (avgVelocity - velocity).normalized * simulation.matchingFactor;
        }
    }

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
        if (transform.position.x > simulation.bounds.x)
        {
            velocity.x -= simulation.turnFactor;
        }
        if (transform.position.x < -simulation.bounds.x)
        {
            velocity.x += simulation.turnFactor;
        }
        if (transform.position.y > simulation.bounds.y)
        {
            velocity.y -= simulation.turnFactor;
        }
        if (transform.position.y < -simulation.bounds.y)
        {
            velocity.y += simulation.turnFactor;
        }
        if (transform.position.z > simulation.bounds.z)
        {
            velocity.z -= simulation.turnFactor;
        }
        if (transform.position.z < -simulation.bounds.z)
        {
            velocity.z += simulation.turnFactor;
        }
    }

    private Fish[] GetFishInRange(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        List<Fish> fishList = new();

        foreach (Collider collider in colliders)
        {
            if (collider != Collider && collider.gameObject.TryGetComponent(out Fish fish))
            {
                fishList.Add(fish);
            }
        }

        return fishList.ToArray();
    }
}
