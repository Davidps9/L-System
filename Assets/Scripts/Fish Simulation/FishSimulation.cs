using System;
using System.Collections.Generic;
using UnityEngine;

public class FishSimulation : MonoBehaviour
{
    [Header("Alignment")]
    [Range(0f, 0.1f)] public float matchingFactor = 0.05f; // alignment
    [Header("Separation")]
    public float minDistance = 1f;
    [Range(0f, 0.1f)] public float avoidFactor = 0.05f; // separation
    [Header("Cohesion")]
    [Range(0f, 0.01f)] public float centeringFactor = 0.005f; // coherence
    [Header("Bounds")]
    public Vector3 bounds = Vector3.one;
    [Range(0, 1)] public float turnFactor = 1;
    [Space]
    public float visualRange = 5;
    public float maxSpeed = 15;
    [SerializeField] private int fishCount = 100;
    [Space]
    [SerializeField] private GameObject fishPrefab;

    [NonSerialized] public List<Fish> fishList = new();


    void Start()
    {
        for (int i = 0; i < fishCount; i++)
        {
            GameObject fish = Instantiate(fishPrefab, transform);
            if (fish.TryGetComponent(out Fish fishScript))
            {
                fishScript.Init(this);
                fishList.Add(fishScript);
                continue;
            }
            Debug.LogError("Fish prefab does not have a Fish component!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, bounds * 2);
    }
}
