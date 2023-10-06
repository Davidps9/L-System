using UnityEngine;

public class Node
{
    public Vector3 localPosition
    {
        get
        {
            if (parent == null)
            {
                return position;
            }
            else
            {
                return WorldToLocalPosition(position);
            }
        }
        set
        {
            if (parent == null)
            {
                position = value;
            }
            else
            {
                position = LocalToWorldPosition(value);
            }
        }
    }
    public Vector3 position;
    public Vector3 localRotation
    {
        get
        {
            if (parent == null)
            {
                return rotation;
            }
            else
            {
                return WorldToLocalRotation(rotation);
            }
        }
        set
        {
            if (parent == null)
            {
                rotation = value;
            }
            else
            {
                rotation = LocalToWorldRotation(value);
            }
        }
    }
    public Vector3 rotation;
    public Node parent;

    public Node(Vector3 position, Vector3 rotation, Node parent = null, bool isLocalPosition = false)
    {
        this.parent = parent;

        if (isLocalPosition)
        {
            localPosition = position;
            localRotation = rotation;
        }
        else
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
    public Node(Node _parent = null, bool isLocalPosition = false) : this(Vector3.zero, Vector3.zero, _parent, isLocalPosition) { }

    Vector3 LocalToWorldPosition(Vector3 localPosition)
    {
        return parent.position + RotatedPosition(localPosition, parent.rotation);
    }

    Vector3 WorldToLocalPosition(Vector3 worldPosition)
    {
        return RotatedPosition(worldPosition - parent.position, parent.rotation);
    }

    Vector3 LocalToWorldRotation(Vector3 localRotation)
    {
        return parent.rotation + localRotation;
    }

    Vector3 WorldToLocalRotation(Vector3 worldRotation)
    {
        return worldRotation - parent.rotation;
    }

    Vector3 RotatedPosition(Vector3 position, Vector3 rotation)
    {
        Quaternion quaternion = Quaternion.Euler(rotation);
        Vector3 result = quaternion * position;
        return result;
    }
}