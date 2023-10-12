using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Branch : FishDetectable
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    [HideInInspector] public List<Node> nodes = new();
    [HideInInspector] public Node lastNode => nodes.Last();

    #region Branch Creation

    public void Initialize(Transform parent, Node rootNode)
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        transform.SetParent(parent);
        transform.localPosition = rootNode.position;

        Node newRootNode = new(Vector3.zero, rootNode.rotation);
        newRootNode.radius = rootNode.radius;
        ApplyNode(newRootNode);

        affectsSeparation = true;
    }

    public void CreateMesh(int sideCount, Material material)
    {
        if (nodes.Count < 1)
        {
            Debug.LogError("Not enough nodes to create a mesh");
            return;
        }

        meshRenderer.material = material;
        Mesh mesh = MeshGenerator.GenerateMesh(nodes.ToArray(), sideCount, "Branch");
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public Node CreateNode(float radius)
    {
        Node node = new(lastNode, true, radius);
        // nodes.Add(node);
        return node;
    }

    public void ApplyNode(Node node)
    {
        nodes.Add(node);
    }

    #endregion

    #region Branch Interaction (WORK IN PROGRESS)

    public float rotationSpeed = 5.0f; // Adjust this value to control the smoothing speed
    private Quaternion targetRotation = Quaternion.identity;

    //void Update()
    //{
    //    // Smoothly interpolate the rotation towards the target rotation
    //    Quaternion currentRotation = transform.localRotation;
    //    float step = rotationSpeed * Time.deltaTime;
    //    transform.localRotation = Quaternion.RotateTowards(currentRotation, targetRotation, step);
    //}

    // Public method to gradually change the rotation speed and update the target rotation
    public void SetTargetRotationWithEase(Quaternion newTargetRotation, float easeSpeed)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeRotationWithEase(newTargetRotation, easeSpeed));
    }

    private IEnumerator ChangeRotationWithEase(Quaternion newTargetRotation, float easeSpeed)
    {
        Quaternion startRotation = transform.localRotation;
        float journeyLength = Quaternion.Angle(startRotation, newTargetRotation);
        float startTime = Time.time;

        while (transform.localRotation != newTargetRotation)
        {
            float distanceCovered = (Time.time - startTime) * easeSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.localRotation = Quaternion.Slerp(startRotation, newTargetRotation, fractionOfJourney);
            yield return null;
        }
    }

    #endregion
}
