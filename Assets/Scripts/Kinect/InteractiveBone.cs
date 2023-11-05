using System;
using UnityEngine;

public class InteractiveBone : MonoBehaviour
{
    [SerializeField] private InteractiveBoneCollider boneCollider;
    [SerializeField] private ParticleSystem particles;

    private float boneLength = 0;
    private Vector3 boneDirection = Vector3.zero;
    private Vector3 boneCenter = Vector3.zero;
    private Vector3 prevBoneCenter = Vector3.zero;
    [NonSerialized] public Vector3 velocity = Vector3.zero;

    //private LineRenderer lineRenderer;

    public void Init(bool ignoreDepth)
    {
        boneCollider.Init(ignoreDepth);
        // DEBUG LINE
        //lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.startWidth = 0.05f;
        //lineRenderer.endWidth = 0.05f;
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.startColor = Color.red;
        //lineRenderer.endColor = Color.red;
        //lineRenderer.positionCount = 2;
    }

    public void Refresh(Vector3 firstPosition, Vector3 secondPosition)
    {
        // debug line
        //lineRenderer.SetPositions(new Vector3[] { firstPosition, secondPosition });

        prevBoneCenter = boneCenter;
        boneCenter = (firstPosition + secondPosition) * 0.5f;
        if (boneCenter != prevBoneCenter)
        {
            velocity = (boneCenter - prevBoneCenter) / Time.deltaTime;
            transform.position = boneCenter;
        }

        boneLength = Vector3.Distance(firstPosition, secondPosition);
        boneDirection = (secondPosition - firstPosition).normalized;

        UpdateParticleSystem();
        boneCollider.Refresh(boneLength, boneCenter, velocity);

    }

    private void UpdateParticleSystem()
    {
        var shapeModule = particles.shape;
        shapeModule.scale = new Vector3(shapeModule.scale.x, shapeModule.scale.y, boneLength);
        shapeModule.rotation = Quaternion.LookRotation(boneDirection, Vector3.forward).eulerAngles;
    }
}
