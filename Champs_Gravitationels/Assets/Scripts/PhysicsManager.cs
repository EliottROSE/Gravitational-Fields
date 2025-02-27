using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class PhysicManager : MonoBehaviour
{
    [HideInInspector] public List<CelestialObject> instantiatedObjects;
    public List<CelestialObject> prefabs;
    public CelestialObject genericPrefab;
    public bool bIgnoreSun = true;

    private Dictionary<Vector3, float> m_originPointsData;

    public List<LineRenderer> lineRenderers;
    public List<Vec3> spherePoints;
    public List<Vec3> startPositions;

    public static bool IsPaused = false;
    private void Start()
    {

        foreach (CelestialObject celestialObjectPrefab in prefabs)
        {
            CelestialObject celestialInstance = Instantiate(celestialObjectPrefab,
                celestialObjectPrefab.originalPosition,
                Quaternion.identity);

            instantiatedObjects.Add(celestialInstance);
        }

        StartCompute();


    }

    private void FixedUpdate()
    {
        if (IsPaused)
            return;
        //Debug.Log("-------------------------------------------------------------------------------------------------------------");
        //Debug.Log("Frame : " + frame);
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            Vector3 pos = NewPosition(celestialObject.AstronomicalPos, celestialObject.msSpeed,
                celestialObject.msAccel);
            celestialObject.AstronomicalPos = pos;

            //Debug.Log("Name : " + celestialObject.name);
            //Debug.Log("New AstronomicalPos" + celestialObject.AstronomicalPos);
            //Debug.Log("New EnginePos : " + celestialObject.transform.position);

            celestialObject.transform.position = VectorToEngine(pos / Constant.AstronomicalDistance);


            celestialObject.oldMsAccel = celestialObject.msAccel;
            celestialObject.msAccel = GravitationalForce(celestialObject);

            celestialObject.msSpeed =
                NewSpeed(celestialObject.msSpeed, celestialObject.oldMsAccel, celestialObject.msAccel);
        }
    }

    public void AddPlanet(Vector3 pos, Vector3 speed, float mass, Vector3 size)
    {
        CelestialObject newPlanet = Instantiate(genericPrefab, VectorToEngine(pos), Quaternion.identity);
        newPlanet.transform.localScale = VectorToEngine(size * Constant.Scale);

        newPlanet.AstronomicalPos = pos * Constant.AstronomicalDistance;
        newPlanet.msSpeed = speed * Constant.KmPerSecToMeterPerSec;
        newPlanet.kgMass = mass * Constant.EarthMass;

        newPlanet.oldMsAccel = Vector3.Zero;
        newPlanet.msAccel = GravitationalForce(newPlanet);

        instantiatedObjects.Add(newPlanet);
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
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            celestialObject.oldMsAccel = Vector3.Zero;
            celestialObject.msAccel = GravitationalForce(celestialObject);
        }
    }

    /*------------------------------------------Base Algorithm--------------------------------*/
    private static Vector3 NewPosition(Vector3 oldPosition, Vector3 oldSpeed, Vector3 oldAcceleration)
    {
        Vector3 result = oldPosition + oldSpeed * Constant.DeltaT +
                         0.5f * oldAcceleration * Mathf.Pow(Constant.DeltaT, 2f);
        return result;
    }

    private static Vector3 Acceleration(Vector3 origin, Vector3 target, float originMass)
    {
        Vector3 distance = origin - target; // Vecteur Rij en 3D

        float distSquared = distance.LengthSquared() + 0.0001f; // Offset to avoid NaN
        float distCubed = Mathf.Sqrt(distSquared) * distSquared;

        Vector3 result = Constant.Gravity * (distance / distCubed) * originMass;

        return result;
    }

    private static Vector3 NewSpeed(Vector3 oldSpeed, Vector3 oldAcceleration, Vector3 newAcceleration)
    {
        Vector3 result = oldSpeed + (oldAcceleration + newAcceleration) * 0.5f * Constant.DeltaT;
        return result;
    }


    /*------------------------------------------Total Gravitational Field--------------------------------*/
    public Vec3 TotalGravitionalField(Vec3 target)
    {
        Vec3 totalGravity = Vec3.zero;

        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            if (bIgnoreSun && celestialObject.objectName.Equals("Sun", StringComparison.OrdinalIgnoreCase))
                continue;

            float mass = celestialObject.kgMass;
            Vec3 direction = celestialObject.transform.position - target;
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
        Vector3 sum = Vector3.Zero;
        Vector3 target = m_celestial.AstronomicalPos;
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            if (m_celestial == celestialObject) continue;

            Vector3 origin = celestialObject.AstronomicalPos;
            sum += Acceleration(origin, target, celestialObject.kgMass);
        }

        return sum;
    }
    public abstract class Constant
    {
        public const float Gravity = 6.6743e-11f; // m3 kg-1 s-2   
        public const float EarthMass = 5.927e24f; // kg
        public const float KmPerSecToMeterPerSec = 1000; // m.s-1
        public const float AstronomicalDistance = 1.495978707e11f; // m
        public const float DeltaT = 36000f; // s
        public const float Scale = 0.1f;
    }
}