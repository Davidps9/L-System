using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObstacleAvoidance : MonoBehaviour
{

    [SerializeField] GameObject prefab;
    [SerializeField] GameObject parent;
    [SerializeField] Material Highlight;
    [Header ("Math Components")]
    [SerializeField] int numOfPoints;
    [SerializeField] float turnFraction;
    [SerializeField] float turnFractionSpeed;
    [SerializeField] float radius;
    [SerializeField] float highlightOffset;
    [SerializeField] float highlightValue;
    [Header ("Montaje truco")]
    [SerializeField] GameObject cameraObj;
    [SerializeField]private float angle;

    private List<GameObject> obstacles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateFOV());
    }

    // Update is called once per frame
    void Update()
    {


    }

    IEnumerator GenerateFOV()
    {
        
        for (int i = 0; i < numOfPoints; i++)
        {
            float t = i / (numOfPoints - 1f);
            float inclination = Mathf.Acos (1 -2 *t);
            float azimuth = 2 * Mathf.PI * turnFraction * i;

            float x =Mathf.Sin(inclination) * Mathf.Cos(azimuth) * radius;
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) * radius;
            float z = Mathf.Cos(inclination) * radius;

            Vector3 pos = new Vector3(x, y, z);
            GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            while(clone.transform.position != pos)
            {
                clone.transform.Translate(pos);
                yield return new WaitForEndOfFrame();
            }
            clone.transform.parent = parent.transform;
            Material mat;
            
            if((i + highlightOffset) % highlightValue == 0)
            {
                mat = Highlight;
                clone.GetComponent<MeshRenderer>().material = mat;

            }
            obstacles.Add(clone);
            yield return new WaitForEndOfFrame();

        }
        prefab.SetActive(false);
        StartCoroutine(CalculatePosition());

    }

    IEnumerator CalculatePosition()
    {
        while (true) { 
            turnFraction += turnFractionSpeed;

            for (int i = 0;i < numOfPoints;i++)
            {
                float t = i / (numOfPoints - 1f);
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = 2 * Mathf.PI * turnFraction * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth) * radius;
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) * radius;
                float z = Mathf.Cos(inclination) * radius;

                Vector3 pos = new Vector3(x, y, z);

                UpdatePsition(i, pos);
               // Debug.DrawRay(parent.transform.position, pos, Color.green);
            }
            yield return new WaitForEndOfFrame();

            cameraObj.transform.RotateAround(parent.transform.position, Vector3.up, angle);
            cameraObj.transform.LookAt(parent.transform.position);
            Debug.Log(angle);
            Debug.Log(cameraObj.transform.rotation);
        }
    }


    void UpdatePsition(int i, Vector3 targetPos)
    {
        
            obstacles[i].transform.localPosition = targetPos;

        
    }

    
}
