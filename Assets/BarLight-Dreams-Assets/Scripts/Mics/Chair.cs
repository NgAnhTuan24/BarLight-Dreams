using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    public Transform sitPoint;

    public SitDirection sitDirection;

    private void Start()
    {
        ChairManager.Instance.RegisterChair(this);
    }

    private void OnDisable()
    {
        if (ChairManager.Instance != null)
        {
            ChairManager.Instance.UnregisterChair(this);
        }
    }

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