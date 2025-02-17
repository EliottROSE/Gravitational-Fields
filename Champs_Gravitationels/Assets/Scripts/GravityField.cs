using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityField : MonoBehaviour
{
    public List<CelestialObject> celestialsBody;
    public float gravitationalConstant = PhysicManager.Constant.Gravity;
    // Start is called before the first frame update
    void Start()
    {
        //Fetch all celestial body in scene
        celestialsBody = new List<CelestialObject>(FindObjectsOfType<CelestialObject>());
    }

    public Vector3 ComputeGravity(Vector3 position)
    {
        Vector3 totalGravity = Vector3.zero;

        foreach(CelestialObject body in celestialsBody)
        {
            Vector3 direction = body.position - position;
            float distance = direction.magnitude;

            if(distance > 0.5f)
            {
                float forceMagnitude = gravitationalConstant * body.kgMass / (distance * distance);
                totalGravity += direction.normalized * forceMagnitude;
            }
        }
        return totalGravity;
    }
}
