using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class FieldLines : MonoBehaviour
{
    PhysicManager physicManager;

    LineRenderer lineRenderer;
    public int linesCount = 50;

    public Vector3 currentPos;

    void Start()
    {
        physicManager = FindAnyObjectByType<PhysicManager>();
        if (physicManager == null)
            Debug.LogError("Cannot find physic manager on celestial object" + this.name);

        lineRenderer = FindAnyObjectByType<LineRenderer>();
        if (lineRenderer == null)
            Debug.LogError("Cannot find line renderer on celestial object" + this.name);

        lineRenderer.positionCount = linesCount;
        currentPos = PhysicManager.VectorToSystem(transform.position);

    }

    void Update()
    {
        DrawFieldLines();
    }

    public void DrawFieldLines()
    {
        //lineRenderer.positionCount = 2;

        //lineRenderer.SetPosition(0, transform.position);
        //Vec3 t = transform.position + new Vec3(0f, 0f, 50f);
        //lineRenderer.SetPosition(0, t);

        //for (int i = 0; i < lineRenderer.positionCount; ++i)
        //{
        //    Vector3 newPos = physicManager.LineFieldNextPos(currentPos * PhysicManager.Constant.AstronomicalDistance, 1f);
        //    currentPos = newPos / PhysicManager.Constant.AstronomicalDistance;
        //    lineRenderer.SetPosition(i, PhysicManager.VectorToEngine(newPos / PhysicManager.Constant.AstronomicalDistance));
        //}
        //currentPos = PhysicManager.VectorToSystem(transform.position);
        Debug.Log("Planet : " + transform.position);
        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            Vector3 newPos = physicManager.LineFieldNextPos(currentPos, 10f);
            Vector3 unityRef = (newPos / PhysicManager.Constant.AstronomicalDistance);
            Debug.Log("pos : " + unityRef);
            lineRenderer.SetPosition(i, PhysicManager.VectorToEngine(unityRef * new Vector3(0f, 0, 50f)/* * 10f*/));
            currentPos = unityRef;
        }
        currentPos = PhysicManager.VectorToSystem(transform.position);
    }
}
