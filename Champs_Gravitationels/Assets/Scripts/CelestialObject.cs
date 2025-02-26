using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;
using System;

public class CelestialObject : MonoBehaviour
{
    public string objectName;
    public Vec3 position = Vec3.zero;
    public Vec3 kmsSpeed = Vec3.zero;
    
    public float mass;

    [NonSerialized] public float kgMass;
    [NonSerialized] public Vector3 msSpeed;

    [NonSerialized] public Vector3 msAccel;
    [NonSerialized] public Vector3 oldMsAccel;
    [NonSerialized] public Vector3 AstronomicalPos;

    [NonSerialized] private Vector3 m_Speed;

    private void Awake()
    {
        m_Speed = PhysicManager.VectorToSystem(kmsSpeed);
        kgMass = mass * PhysicManager.Constant.EarthMass;
        msSpeed = m_Speed * PhysicManager.Constant.KmPerSecToMeterPerSec;
        AstronomicalPos = PhysicManager.VectorToSystem(position * PhysicManager.Constant.AstronomicalDistance);
    }

    private void FixedUpdate()
    {
        m_Speed = PhysicManager.VectorToSystem(kmsSpeed);
        transform.Rotate(new Vec3(0f, 0.1f, 0f));
    }
}
