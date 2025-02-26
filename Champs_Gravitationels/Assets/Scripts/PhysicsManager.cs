using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class PhysicManager : MonoBehaviour
{
    [HideInInspector] public List<CelestialObject> instantiatedObjects;
    public List<CelestialObject> prefabs;
    public bool bIgnoreSun = true;

    private Dictionary<Vector3, float> m_originPointsData;

    private void Start()
    {
        foreach (var celestialObjectPrefab in prefabs)
        {
            var celestialInstance = Instantiate(celestialObjectPrefab, celestialObjectPrefab.position * 10f,
                Quaternion.identity);

            instantiatedObjects.Add(celestialInstance);
        }

        StartCompute();
    }

    private void FixedUpdate()
    {
        //Debug.Log("-------------------------------------------------------------------------------------------------------------");
        //Debug.Log("Frame : " + frame);
        foreach (var celestialObject in instantiatedObjects)
        {
            var pos = NewPosition(celestialObject.AstronomicalPos, celestialObject.msSpeed, celestialObject.msAccel);
            celestialObject.AstronomicalPos = pos;

            //Debug.Log("Name : " + celestialObject.name);
            //Debug.Log("New AstronomicalPos" + celestialObject.AstronomicalPos);
            //Debug.Log("New EnginePos : " + celestialObject.transform.position);

            celestialObject.transform.position = VectorToEngine(pos / Constant.AstronomicalDistance) * 10f;


            celestialObject.oldMsAccel = celestialObject.msAccel;
            celestialObject.msAccel = GravitationalForce(celestialObject);

            celestialObject.msSpeed =
                NewSpeed(celestialObject.msSpeed, celestialObject.oldMsAccel, celestialObject.msAccel);
        }
    }

    public static Vector3 VectorToSystem(Vec3 unityVec)
    {
        return new Vector3(unityVec.x, unityVec.y, unityVec.z);
    }

    public static Vec3 VectorToEngine(Vector3 systemVec)
    {
        return new Vec3(systemVec.X, systemVec.Y, systemVec.Z);
    }

    private void StartCompute()
    {
        foreach (var celestialObject in instantiatedObjects)
        {
            // Do not need to compute pos, precalculate
            celestialObject.oldMsAccel = Vector3.Zero;
            celestialObject.msAccel = GravitationalForce(celestialObject); // compute initial acceleration
            //Debug.Log(celestialObject.name);
            //Debug.Log("Start data");
            //Debug.Log("AstronomicalPos : " + celestialObject.AstronomicalPos);
            //Debug.Log("EnginePos : " + celestialObject.transform.position);
            //Debug.Log("old : " + celestialObject.oldMsAccel);
            //Debug.Log("Accel : " + celestialObject.msAccel);
            //Debug.Log("speed : " + celestialObject.msSpeed);
        }
    }

    /*------------------------------------------Base Algorithm--------------------------------*/
    private static Vector3 NewPosition(Vector3 oldPosition, Vector3 oldSpeed, Vector3 oldAcceleration)
    {
        var result = oldPosition + oldSpeed * Constant.DeltaT + 0.5f * oldAcceleration * Mathf.Pow(Constant.DeltaT, 2f);
        return result;
    }

    private static Vector3 Acceleration(Vector3 origin, Vector3 target, float originMass)
    {
        //Vector3 distance = origin - target; // vector Rij

        //Vector3 result = Constant.Gravity * (distance / Mathf.Pow(distance.Length(), 3)) * originMass;

        //return result;
        var distance = origin - target; // Vecteur Rij en 3D

        // Assurez-vous de ne pas avoir de division par z�ro, m�me avec de petites distances
        float distSquared = distance.LengthSquared() + 0.0001f; // Offset pour �viter NaN
        float distCubed = Mathf.Sqrt(distSquared) * distSquared; // Calcul du cube de la distance

        // Calcul de la force gravitationnelle en prenant en compte la distance 3D
        var result = Constant.Gravity * (distance / distCubed) * originMass;

        return result;
    }

    private static Vector3 NewSpeed(Vector3 oldSpeed, Vector3 oldAcceleration, Vector3 newAcceleration)
    {
        var result = oldSpeed + (oldAcceleration + newAcceleration) * 0.5f * Constant.DeltaT;
        return result;
    }


    /*------------------------------------------Total Gravitational Field--------------------------------*/
    public Vec3 TotalGravitionalField(Vec3 target)
    {
        var totalGravity = Vec3.zero;

        foreach (var celestialObject in instantiatedObjects)
        {
            if (bIgnoreSun && celestialObject.objectName.Equals("Sun", StringComparison.OrdinalIgnoreCase))
                continue;

            float mass = celestialObject.kgMass;
            var direction = celestialObject.transform.position - target;
            float distanceSqr = direction.magnitude;

            if (!(distanceSqr > 0.001f)) continue; // Avoid 0 divisions

            float forceMagnitude = Constant.Gravity * mass / distanceSqr;
            totalGravity += direction.normalized * forceMagnitude;
        }

        return totalGravity;
    }


    /*------------------------------------------Gravitational Acceleration------------------------------------------*/
    private Vector3 GravitationalForce(CelestialObject m_celestial)
    {
        var sum = Vector3.Zero;
        var target = m_celestial.AstronomicalPos;
        foreach (var celestialObject in instantiatedObjects)
        {
            if (m_celestial == celestialObject) continue;

            var origin = celestialObject.AstronomicalPos;
            sum += Acceleration(origin, target, celestialObject.kgMass);
        }

        return sum;
    }

    /*------------------------------------------Line field------------------------------------------*/
    public Vector3 LineFieldNextPos(Vector3 currentPos, float step)
    {
        var currentAstronomicalPos = currentPos * Constant.AstronomicalDistance;

        var gravitationalField = VectorToSystem(-TotalGravitionalField(VectorToEngine(currentAstronomicalPos)));

        if (gravitationalField.Length() < 0.001f)
            return Vector3.Zero;

        currentAstronomicalPos +=
            step * Constant.AstronomicalDistance * gravitationalField / gravitationalField.Length();

        return currentAstronomicalPos; /* In astronomical reference */
    }

    public abstract class Constant
    {
        public const float Gravity = 6.6743e-11f; // m3 kg-1 s-2   
        public const float EarthMass = 5.927e24f; // kg
        public const float KmPerSecToMeterPerSec = 1000; // m.s-1
        public const float AstronomicalDistance = 1.495978707e11f; // m
        public const float DeltaT = 36000f; // s
    }
}