using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class PhysicManager : MonoBehaviour
{
    public class Constant
    {
        public const float Gravity = 6.6743e-11f; // N m2 kg-2   
        public const float EarthMass = 5.927e24f; // kg
        public const float AstronomicalSpeed = 29.78f; // k.s-1
        public const float AstronomicalDistance = 1.495978707e11f; // m
        public const float deltaT = 3600f; // s
    }
    Dictionary<Vector3, float> originPointsData;

    [SerializeField] public List<CelestialObject> prefabs;

    public List<CelestialObject> instantiatedObjects;

    static public Vector3 VectorToSystem(Vec3 unityVec)
    {
        return new Vector3(unityVec.x, unityVec.y, unityVec.z);
    }
    static public Vec3 VectorToEngine(Vector3 systemVec)
    {
        return new Vec3(systemVec.X, systemVec.Y, systemVec.Z);
    }

    void Start()
    {
        foreach (CelestialObject celestialObjectprefab in prefabs)
        {
            CelestialObject celestialInstance = Instantiate(celestialObjectprefab, celestialObjectprefab.position, Quaternion.identity); 
            // Do not need to compute pos, precalculate
            
            celestialInstance.oldMsAccel = Vector3.Zero;
            celestialInstance.msAccel = GravitationalForce(celestialInstance); // compute initial acceleration 

            // Do not need to compute speed, precalculate

            instantiatedObjects.Add(celestialInstance);



            if (celestialInstance.name == "Earth Physics Variant(Clone)")
            {
                Debug.Log("Start data");
                Debug.Log("AstronomicalPos : " + celestialInstance.AstronomicalPos);
                Debug.Log("EnginePos : " + celestialInstance.transform.position);
                Debug.Log("old : " + celestialInstance.oldMsAccel);
                Debug.Log("Accel : " + celestialInstance.msAccel);
            }

        //Debug.Log("Instancing");
        //Debug.Log(celestialInstance.name);
        //Debug.Log("speed " + celestialInstance.msSpeed);
        //Debug.Log("pos " + celestialInstance.AstronomicalPos);
        //Debug.Log("mass " + celestialInstance.kgMass);
        }
    }

    void FixedUpdate()
    {
        foreach (CelestialObject celestialObject in instantiatedObjects)
        {
            Vector3 pos = NewPosition(celestialObject.AstronomicalPos, celestialObject.msSpeed, celestialObject.msAccel, Constant.deltaT);
            celestialObject.AstronomicalPos = pos;

            Debug.Log("New AstronomicalPos" + celestialObject.AstronomicalPos);

            celestialObject.transform.position = VectorToEngine(pos / Constant.AstronomicalDistance);
            Debug.Log("New EnginePos : " + celestialObject.transform.position);


            celestialObject.oldMsAccel = celestialObject.msAccel;
            celestialObject.msAccel = GravitationalForce(celestialObject);

            celestialObject.msSpeed = NewSpeed(celestialObject.msSpeed, celestialObject.oldMsAccel, celestialObject.msAccel, Constant.deltaT);

        }
    }

    /*------------------------------------------Base Algorithm--------------------------------*/
    public Vector3 NewPosition(Vector3 oldPosition, Vector3 oldSpeed, Vector3 oldAcceleration, float deltaT)
    {
        Vector3 result = oldPosition + (oldSpeed * deltaT) + (0.5f * oldAcceleration * Mathf.Pow(deltaT, 2));
        return result;
    }

    public Vector3 Acceleration(Vector3 origin, Vector3 target, float originMass)
    {
        Vector3 distance = target - origin; // vector Rij

        Vector3 result = Constant.Gravity * (distance / Mathf.Pow(distance.Length(), 3)) * originMass;

        return result;
    }

    public Vector3 NewSpeed(Vector3 oldSpeed, Vector3 oldAcceleration, Vector3 newAcceleration, float deltaT)
    {
        Vector3 result = oldSpeed + ((oldAcceleration + newAcceleration) * 0.5f) * deltaT; 
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
}
