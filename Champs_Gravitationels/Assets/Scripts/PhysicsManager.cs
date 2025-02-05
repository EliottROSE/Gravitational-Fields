using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;

public class Constant
{
    public const float Gravity = 6.6743e-11f;
}

public class PhysicManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        // t + delta T = ACCUMALATION DU TEMPS
    }

    /*------------------------------------------Base Algorithm--------------------------------*/
    Vector3 NewPosition(Vector3 oldPosition, Vector3 oldSpeed, Vector3 oldAcceleration, float deltaT)
    {
        Vector3 result = oldPosition + oldSpeed * deltaT +  0.5f * oldAcceleration * Mathf.Pow(deltaT, 2);
        return result;
    }

    Vector3 Acceleration(Vector3 origin, Vector3 target, float originMass)
    {
        Vector3 distance = target - origin; // vector Rij

        Vector3 result = Constant.Gravity * (distance / Mathf.Pow(distance.Length(), 3)) * originMass;

        return result;
    }

    Vector3 NewSpeed(Vector3 oldSpeed, Vector3 oldAcceleration, Vector3 newAcceleration, float deltaT)
    {
        Vector3 result = oldSpeed + ((oldAcceleration + newAcceleration) * 0.5f) * deltaT; 
        return result;
    }


    /*------------------------------------------Total Gravitational Field--------------------------------*/
    Vector3 TotalGravitionalField(Vector3 target, Dictionary<Vector3, float> originPointsData)
    {
        Vector3 sum = Vector3.Zero;
        foreach (KeyValuePair<Vector3, float> data in originPointsData)
        {
            sum += Acceleration(data.Key, target, data.Value);
        }
        return sum;
    }


    /*------------------------------------------Gravitational Acceleration------------------------------------------*/
    Vector3 GravitationalForce(Vector3 target, Dictionary<Vector3, float> originPointsData)
    {
        Vector3 sum = Vector3.Zero;
        foreach (KeyValuePair<Vector3, float> data in originPointsData)
        {
            if (target !=data.Key)
                sum += Acceleration(data.Key, target, data.Value);
        }
        return sum;
    }
}
