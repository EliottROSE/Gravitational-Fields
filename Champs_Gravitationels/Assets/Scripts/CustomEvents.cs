using System;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{

    public static event Action OnCelestialObjectClicked;

    public static void ObjectClicked()
    {
        OnCelestialObjectClicked?.Invoke();
    }
}
