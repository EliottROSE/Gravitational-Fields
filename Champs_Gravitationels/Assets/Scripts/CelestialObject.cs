using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;
using System;

public class CelestialObject : MonoBehaviour
{
    public string objectName = null;
    [SerializeField] float mass = 0f;
    [SerializeField] float speed = 0f;
    [SerializeField] public Vec3 position = Vec3.zero;
    //[SerializeField] String name;


    [NonSerialized] public float kgMass;
    public Vector3 msSpeed;

    public Vector3 msAccel;
    public Vector3 oldMsAccel;

    public Vector3 AstronomicalPos;

    private void Awake()
    {
        kgMass = mass * PhysicManager.Constant.EarthMass;

        msSpeed = new Vector3(1f, 0f, 0f) * speed * PhysicManager.Constant.MeterPerSecToKmPerSec;

        AstronomicalPos = PhysicManager.VectorToSystem(position * PhysicManager.Constant.AstronomicalDistance);

        //Debug.Log(name);
        //Debug.Log("speed " + msSpeed);
        //Debug.Log("pos " + AstronomicalPos);
        //Debug.Log("mass " + kgMass);
    }
}
