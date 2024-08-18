using UnityEngine;

public class WFC_Node
{
    public bool isPlaceable;
    public Vector3 cellPosition;
    public Transform obj;

    public WFC_Node(bool isPlaceable, Vector3 cellPosition, Transform obj)
    {
        this.isPlaceable = isPlaceable;
        this.cellPosition = cellPosition;
        this.obj = obj;
    }
}