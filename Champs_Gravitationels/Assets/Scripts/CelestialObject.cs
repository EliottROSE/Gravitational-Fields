using System;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class CelestialObject : MonoBehaviour
{
    public string objectName;
    [FormerlySerializedAs("position")] public Vec3 originalPosition = Vec3.zero;
    [SerializeField] public Vec3 kmsSpeed = Vec3.zero;

    public float mass;
    [NonSerialized] public Vector3 AstronomicalPos;

    [NonSerialized] public float kgMass;

    [NonSerialized] private Vector3 m_Speed;

    [NonSerialized] public Vector3 msAccel;
    [NonSerialized] public Vector3 msSpeed;
    [NonSerialized] public Vector3 oldMsAccel;

    private void Awake()
    {
        m_Speed = PhysicManager.VectorToSystem(kmsSpeed);
        kgMass = mass * PhysicManager.Constant.EarthMass;
        msSpeed = m_Speed * PhysicManager.Constant.KmPerSecToMeterPerSec;
        AstronomicalPos = PhysicManager.VectorToSystem(originalPosition * PhysicManager.Constant.AstronomicalDistance);
    }

    private void FixedUpdate()
    {
        if (PhysicManager.IsPaused) return;
        m_Speed = PhysicManager.VectorToSystem(kmsSpeed);
        transform.Rotate(new Vec3(0f, 0.1f, 0f));
    }
}