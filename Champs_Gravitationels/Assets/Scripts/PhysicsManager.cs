using System.Collections.Generic;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class PhysicManager : MonoBehaviour
{
    public class Constant
    {
        public const float Gravity = 6.6743e-11f; // m3 kg-1 s-2   
        public const float EarthMass = 5.927e24f; // kg
        public const float MeterPerSecToKmPerSec = 1000; // m.s-1
        public const float AstronomicalDistance = 1.495978707e11f; // m
        public const float DeltaT = 36000f; // s
    }
    Dictionary<Vector3, float> originPointsData;
    int frame = 0;
    [SerializeField] public List<CelestialObject> prefabs;

    public List<CelestialObject> instantiatedObjects;

    public static Vector3 VectorToSystem(Vec3 unityVec)
    {
        return new Vector3(unityVec.x, unityVec.y, unityVec.z);
    }
    public static Vec3 VectorToEngine(Vector3 systemVec)
    {
        return new Vec3(systemVec.X, systemVec.Y, systemVec.Z);
    }

    void Start()
    {
        foreach (CelestialObject celestialObjectprefab in prefabs)
        {
            CelestialObject celestialInstance = Instantiate(celestialObjectprefab, celestialObjectprefab.position * 10f, Quaternion.identity);

            instantiatedObjects.Add(celestialInstance);
        }
        StartCompute();
    }

    void StartCompute()
    {
        foreach (CelestialObject celestialObject in instantiatedObjects)
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

    void FixedUpdate()
    {
        frame++;
        //Debug.Log("-------------------------------------------------------------------------------------------------------------");
        //Debug.Log("Frame : " + frame);
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            Vector3 pos = NewPosition(celestialObject.AstronomicalPos, celestialObject.msSpeed, celestialObject.msAccel);
            celestialObject.AstronomicalPos = pos;

            //Debug.Log("Name : " + celestialObject.name);
            //Debug.Log("New AstronomicalPos" + celestialObject.AstronomicalPos);
            //Debug.Log("New EnginePos : " + celestialObject.transform.position);

            celestialObject.transform.position = VectorToEngine(pos / Constant.AstronomicalDistance) * 10f;


            celestialObject.oldMsAccel = celestialObject.msAccel;
            celestialObject.msAccel = GravitationalForce(celestialObject);

            celestialObject.msSpeed = NewSpeed(celestialObject.msSpeed, celestialObject.oldMsAccel, celestialObject.msAccel);
        }
    }

    /*------------------------------------------Base Algorithm--------------------------------*/
    public Vector3 NewPosition(Vector3 oldPosition, Vector3 oldSpeed, Vector3 oldAcceleration)
    {
        Vector3 result = oldPosition + oldSpeed * Constant.DeltaT + 0.5f * oldAcceleration * Mathf.Pow(Constant.DeltaT, 2f);
        return result;
    }

    public Vector3 Acceleration(Vector3 origin, Vector3 target, float originMass)
    {
        Vector3 distance = origin - target; // vector Rij

        Vector3 result = Constant.Gravity * (distance / Mathf.Pow(distance.Length(), 3)) * originMass;

        return result;
    }

    public Vector3 NewSpeed(Vector3 oldSpeed, Vector3 oldAcceleration, Vector3 newAcceleration)
    {
        Vector3 result = oldSpeed + ((oldAcceleration + newAcceleration) * 0.5f) * Constant.DeltaT; 
        return result;
    }


    /*------------------------------------------Total Gravitational Field--------------------------------*/
    public Vector3 TotalGravitionalField(Vector3 target)
    {
        Vector3 sum = Vector3.Zero;
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            Vector3 origin = celestialObject.AstronomicalPos;
            float originMass = celestialObject.kgMass;
            sum += Acceleration(origin, target, originMass);
        }
        return sum;
    }


    /*------------------------------------------Gravitational Acceleration------------------------------------------*/
    public Vector3 GravitationalForce(CelestialObject m_celestial)
    {
        Vector3 sum = Vector3.Zero;
        Vector3 target = m_celestial.AstronomicalPos;
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            if (m_celestial != celestialObject)
            {
                Vector3 origin = celestialObject.AstronomicalPos;
                sum += Acceleration(origin, target, celestialObject.kgMass);
            }
        }
        return sum;
    }

    /*------------------------------------------Line field------------------------------------------*/
    public Vector3 LineFieldNextPos(Vector3 currentPos, float step)
    {
        Vector3 currentPosAstro = currentPos * PhysicManager.Constant.AstronomicalDistance;
        Vector3 resultPos = currentPosAstro;

        Vector3 GravitationalField = (TotalGravitionalField(currentPosAstro)) /*/ TotalGravitionalField(currentPosAstro).Length()) * step*/;
        resultPos += GravitationalField * step;

        return resultPos; /* In astronomical reference */
    }
}
