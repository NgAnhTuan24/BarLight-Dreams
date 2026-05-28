using System.Collections.Generic;
using UnityEngine;

public class ChairManager : MonoBehaviour
{
    public static ChairManager Instance { get; private set; }

    private List<Chair> chairs = new List<Chair>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetChairCount()
    {
        return chairs.Count;
    }

    public void RegisterChair(Chair chair)
    {
        if (!chairs.Contains(chair))
        {
            chairs.Add(chair);
        }
    }

    public void UnregisterChair(Chair chair)
    {
        if (chairs.Contains(chair))
        {
            chairs.Remove(chair);
        }
    }

    public Chair GetAvailableChair()
    {
        foreach (Chair chair in chairs)
        {
            if (!chair.IsOccupied)
            {
                return chair;
            }
        }

        return null;
    }
}