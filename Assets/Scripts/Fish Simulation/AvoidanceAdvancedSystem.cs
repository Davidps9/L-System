using System.Collections.Generic;
using UnityEngine;

public class AvoidanceAdvancedSystem : MonoBehaviour
{
    [Header("Math Components")]
    [SerializeField] int numOfPoints;
    [SerializeField] float turnFraction;
    [SerializeField] float fovOffset;
    public float positionDistance;
    [HideInInspector] public List<Vector3> pos = new List<Vector3>();

    void Start()
    {
        GenerateFOV();
    }

    void GenerateFOV()
    {

        for (int i = 0; i < numOfPoints; i++)
        {
            float t = i * fovOffset / (numOfPoints - 1f);
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = 2 * Mathf.PI * turnFraction * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = -Mathf.Cos(inclination);

            pos.Add(new Vector3(x, y, z));

        }
    }

    public Vector3 GetFreeDirection()
    {
        for (int i = 0; i < pos.Count; i++)
        {
            Vector3 dir = transform.TransformDirection(pos[i]);
            Ray ray = new Ray(transform.position, dir);
            if (!Physics.SphereCast(ray, 0.01f, positionDistance * 5, 0))
            {
                Debug.DrawRay(transform.position, dir, Color.red);

                return dir;
            }
        }
        return transform.forward;
    }
    public bool IsHeadingForCollision()
    {
        if (Physics.SphereCast(transform.position, 0.1f, transform.forward, out _, positionDistance, 0))
        {
            return true;
        }
        return false;
    }
}
