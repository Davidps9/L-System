using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CylinderBetweenTwoPoints : MonoBehaviour
{
    [SerializeField]
    private Transform cylinderPrefab;

    private GameObject bottomSphere;
    private GameObject topSphere;
    private GameObject cylinder;

    private void Start()
    {
        bottomSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        topSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bottomSphere.transform.position = new Vector3(0, -2, 0);
        topSphere.transform.position = new Vector3(10, 2, 1);

        InstantiateCylinder(cylinderPrefab, bottomSphere.transform.position, topSphere.transform.position);
    }

    private void Update()
    {
        //leftSphere.transform.position = new Vector3(-1, -2f * Mathf.Sin(Time.time), 0);
        //rightSphere.transform.position = new Vector3(1, 2f * Mathf.Sin(Time.time), 0);

        //UpdateCylinderPosition(cylinder, leftSphere.transform.position, rightSphere.transform.position);
    }

    private void InstantiateCylinder(Transform cylinderPrefab, Vector3 beginPoint, Vector3 endPoint)
    {
        cylinder = Instantiate<GameObject>(cylinderPrefab.gameObject, beginPoint, Quaternion.identity);
        UpdateCylinderPosition(cylinder, beginPoint, endPoint);
    }

    private void UpdateCylinderPosition(GameObject cylinder, Vector3 beginPoint, Vector3 endPoint)
    {
        Vector3 offset = endPoint - beginPoint;
        //Vector3 position = beginPoint + (offset / 2.0f);

        //cylinder.transform.position = position;
        cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, offset);
        Vector3 localScale = cylinder.transform.localScale;
        localScale.y = (endPoint - beginPoint).magnitude / 2;
        cylinder.transform.localScale = localScale;
    }
}
