using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class Fish : FishDetectable
{
    private FishSimulation simulation;
    private List<FishDetectable> fishInRange = new();
    AvoidanceAdvancedSystem script;
    public void Init(FishSimulation simulation)
    {
        script = GetComponent<AvoidanceAdvancedSystem>();

        affectsAlignment = true;
        affectsSeparation = true;
        affectsCohesion = true;

        this.simulation = simulation;
        velocity = Random.insideUnitSphere * simulation.maxSpeed;
    }

    private void Update()
    {
        
        DetectFish();
        //FlyTowardsCenter();
        //AvoidOthers();
        //MatchVelocity();
        //LimitSpeed();
        KeepWithinBounds();
        if (IsHeadingForCollision())
        {
            velocity = ObstacleRays().normalized * simulation.maxSpeed;
        }
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    void DetectFish()
    {
        fishInRange.Clear();
        RaycastHit[] hits = script.DetectObstacles();
        if(hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider.gameObject.TryGetComponent<Fish>(out Fish fish))
                {
                    if(fish != this)
                    {
                        fishInRange.Add(fish);
                        Debug.DrawRay(transform.position, fish.transform.position * script.positionDistance, Color.green);
                    }
                }
               
            }
        }
    }
    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = script.pos.ToArray();

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(transform.position, dir);
            if (!Physics.SphereCast(ray, 0.01f, script.positionDistance *5))
            {
                Debug.DrawRay(transform.position, dir, Color.red);

                return dir;
            }
        }
        return transform.forward;
    }
    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.1f, transform.forward, out hit, script.positionDistance))
        {
            return true;
        }
        return false;
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

    public void AvoidPoint(Vector3 point, float avoidFactor)
    {
        velocity += (transform.position - point).normalized * avoidFactor;
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
        //if (transform.position.y < simulation.transform.position.y - simulation.bounds.y)
        //{
        //    velocity.y += simulation.turnFactor;
        //}
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

    
}
