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
    public float radius;

    public Node(Vector3 position, Vector3 rotation, Node parent = null, bool isLocalPosition = false, float radius = 1)
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

        this.radius = radius;
    }
    public Node(Node _parent = null, bool isLocalPosition = false, float radius = 1) : this(Vector3.zero, Vector3.zero, _parent, isLocalPosition, radius) { }

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

    public static Vector3 RotatedPosition(Vector3 position, Vector3 rotation)
    {
        Quaternion quaternion = Quaternion.Euler(rotation);
        Vector3 result = quaternion * position;
        return result;
    }

    public void SetParent(Node parent, bool keepWorldPosition)
    {
        this.parent = parent;
        if (!keepWorldPosition)
        {
            localPosition = position;
        }
    }

    public static Node Zero => new Node(Vector3.zero, Vector3.zero);

    public static Node FromTransform(Transform transform)
    {
        return NodeExtensions.FromTransform(transform);
    }

    public static Node LocalFromTransform(Transform transform)
    {
        return NodeExtensions.LocalFromTransform(transform);
    }
}

public static class NodeExtensions
{
    public static Node FromTransform(Transform transform)
    {
        Node node = new();
        node.position = transform.position;
        node.rotation = transform.rotation.eulerAngles;
        return node;
    }

    public static Transform FromNode(this Transform transform, Node node)
    {
        transform.position = node.position;
        transform.rotation = Quaternion.Euler(node.rotation);
        return transform;
    }

    public static Node LocalFromTransform(Transform transform)
    {
        Node node = new();
        node.localPosition = transform.localPosition;
        node.localRotation = transform.localRotation.eulerAngles;
        return node;
    }

    public static Transform LocalFromNode(this Transform transform, Node node)
    {
        transform.localPosition = node.localPosition;
        transform.localRotation = Quaternion.Euler(node.localRotation);
        return transform;
    }

    public static Node Clone(this Node node)
    {
        return new Node(node.position, node.rotation, node.parent);
    }
}