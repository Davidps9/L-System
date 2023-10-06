using UnityEngine;

public class Branch
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
                return worldToLocalPosition(position);
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
                position = localToWorldPosition(value);
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
                return worldToLocalRotation(rotation);
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
                rotation = localToWorldRotation(value);
            }
        }
    }
    public Vector3 rotation;
    public Branch parent;

    public Branch(Vector3 position, Vector3 rotation, Branch parent = null, bool isLocalPosition = false)
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
    public Branch(Branch _parent = null, bool isLocalPosition = false) : this(Vector3.zero, Vector3.zero, _parent, isLocalPosition) { }

    Vector3 localToWorldPosition(Vector3 localPosition)
    {
        return parent.position + rotatedPosition(localPosition, parent.rotation);
    }

    Vector3 worldToLocalPosition(Vector3 worldPosition)
    {
        return rotatedPosition(worldPosition - parent.position, parent.rotation);
    }

    Vector3 localToWorldRotation(Vector3 localRotation)
    {
        return parent.rotation + localRotation;
    }

    Vector3 worldToLocalRotation(Vector3 worldRotation)
    {
        return worldRotation - parent.rotation;
    }

    Vector3 rotatedPosition(Vector3 position, Vector3 rotation)
    {
        Quaternion quaternion = Quaternion.Euler(rotation);
        Vector3 result = quaternion * position;
        return result;
    }
}