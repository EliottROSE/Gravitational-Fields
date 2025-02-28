using System;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{
    public static event Action OnCelestialObjectClicked;
    public static event Action OnGridCreated;
    public static event Action OnGridDestroyed;

    public static void ObjectClicked()
    {
        OnCelestialObjectClicked?.Invoke();
    }

    public static void GridCreation()
    {
        OnGridCreated?.Invoke();
    }

    public static void GridDeletion()
    {
        OnGridDestroyed?.Invoke();
    }
}