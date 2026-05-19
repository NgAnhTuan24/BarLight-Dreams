using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    public Transform sitPoint;

    public SitDirection sitDirection;

    public void Occupy()
    {
        IsOccupied = true;
    }

    public void Leave()
    {
        IsOccupied = false;
    }
}

public enum SitDirection
{
    Left,
    Right
}