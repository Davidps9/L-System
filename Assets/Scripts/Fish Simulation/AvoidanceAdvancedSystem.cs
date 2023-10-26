using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceAdvancedSystem : MonoBehaviour
{
    //[SerializeField] GameObject prefab;
    private GameObject parent;
    //[SerializeField] Material Highlight;
    [Header("Math Components")]
    [SerializeField] int numOfPoints;
    [SerializeField] float turnFraction;
    [SerializeField] float fovOffset;
    public float positionDistance;
    [HideInInspector]public List<Vector3> pos = new List<Vector3>();
    //[SerializeField] float highlightOffset;
    //[SerializeField] float highlightValue;
    //[SerializeField] float powPower;
    //private List<GameObject> obstacles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject;
        GenerateFOV();
    }

    void GenerateFOV()
    {

        for (int i = 0; i < numOfPoints ; i++)
        {
            float t = i*fovOffset / (numOfPoints - 1f);
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = 2 * Mathf.PI * turnFraction * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = -Mathf.Cos(inclination);

            pos.Add(new Vector3(x, y, z));

        }
    }

    public RaycastHit[] DetectObstacles()
    {
        List<RaycastHit> rays = new List<RaycastHit>();
        for (int i = 0; i < numOfPoints; i++)
        {

            if(Physics.Raycast(parent.transform.position, pos[i], out RaycastHit hit, positionDistance))
            {
                rays.Add(hit);

            }
            
        }
        return rays.ToArray();
    }

}
