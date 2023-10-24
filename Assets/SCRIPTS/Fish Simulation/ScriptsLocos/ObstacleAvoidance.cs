using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{

    [SerializeField] GameObject prefab;
    [SerializeField] GameObject parent;
    [SerializeField] Material Highlight;
    [Header ("Math Components")]
    [SerializeField] int numOfPoints;
    [SerializeField] float turnFraction;
    [SerializeField] float positionDistance;
    [SerializeField] float highlightOffset;
    [SerializeField] float highlightValue;
    [SerializeField] float powPower;
    private List<GameObject> obstacles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateFOV();
    }

    // Update is called once per frame
    void Update()
    {
        turnFraction += 0.000001f;
        CalculatePosition();
    }

    void GenerateFOV()
    {
        
        for (int i = 0; i < numOfPoints; i++)
        {
            float t = i / (numOfPoints - 1f);
            float inclination = Mathf.Acos (1 -2 *t);
            float azimuth = 2 * Mathf.PI * turnFraction * i;

            float x =Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            Vector3 pos = new Vector3(x, y, z);
            GameObject clone = Instantiate(prefab, pos, Quaternion.identity);
            clone.transform.parent = parent.transform;
            Material mat;
            
            if((i + highlightOffset) % highlightValue == 0)
            {
                mat = Highlight;
                clone.GetComponent<MeshRenderer>().material = mat;

            }
            obstacles.Add(clone);


        }
        prefab.SetActive(false);
    }

    void CalculatePosition()
    {
        for(int i = 0;i < numOfPoints;i++)
        {
            float t = i / (numOfPoints - 1f);
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = 2 * Mathf.PI * turnFraction * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth) * positionDistance;
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) * positionDistance;
            float z = Mathf.Cos(inclination) * positionDistance;

            Vector3 pos = new Vector3(x, y, z);

            UpdatePsition(i, pos);
        }
    }


    void UpdatePsition(int i, Vector3 targetPos)
    {
        
            obstacles[i].transform.position = targetPos;

        
    }
}
