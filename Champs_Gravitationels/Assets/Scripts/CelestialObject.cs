using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;
using System;

public class CelestialObject : MonoBehaviour
{
    public string objectName;
    public Vec3 position = Vec3.zero;
    
    public float mass;
    [SerializeField] private float speed;

    [NonSerialized] public float kgMass;
    public Vector3 msSpeed;

    public Vector3 msAccel;
    public Vector3 oldMsAccel;
    public Vector3 AstronomicalPos;
    
    // Proper values to display
    [HideInInspector] public Vector3 kmsSpeed;

    private void Awake()
    {
        kgMass = mass * PhysicManager.Constant.EarthMass;
        msSpeed = new Vector3(1f, 0f, 0f) * speed * PhysicManager.Constant.MeterPerSecToKmPerSec;
        AstronomicalPos = PhysicManager.VectorToSystem(position * PhysicManager.Constant.AstronomicalDistance);
    }

    private void LateUpdate()
    {
        transform.Rotate(new Vec3(0f, 0.1f, 0f));
        //UpdatedAstronomicalPos = PhysicManager.VectorToSystem(position * PhysicManager.Constant.AstronomicalDistance);
        kmsSpeed = new Vector3(1f, 0f, 0f) * speed * PhysicManager.Constant.MeterPerSecToKmPerSec / 1000f;
    }
}
