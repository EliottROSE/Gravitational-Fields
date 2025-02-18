using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class FieldLines : MonoBehaviour
{
    PhysicManager physicManager;

    LineRenderer lineRenderer;
    public int linesCount = 150;
    public float step = 0.01f;
    public Vector3 startPos;
    Vec3 value;

    void Start()
    {
        physicManager = FindAnyObjectByType<PhysicManager>();
        if (physicManager == null)
            Debug.LogError("Cannot find physic manager on celestial object" + this.name);

        lineRenderer = FindAnyObjectByType<LineRenderer>();
        if (lineRenderer == null)
            Debug.LogError("Cannot find line renderer on celestial object" + this.name);

        lineRenderer.positionCount = linesCount;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;
        startPos = PhysicManager.VectorToSystem(transform.position + UnityEngine.Random.onUnitSphere * transform.localScale.x);
        value = UnityEngine.Random.onUnitSphere;


    }

    void Update()
    {
        DrawFieldLines();
    }

    public void DrawFieldLines()
    {
        startPos = PhysicManager.VectorToSystem(transform.position + value * transform.localScale.x);
        List<Vec3> positions = new List<Vec3>();

        Vector3 currentPos = startPos;
        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            positions.Add(PhysicManager.VectorToEngine(currentPos));

            Vector3 newPos = (physicManager.LineFieldNextPos(currentPos /10f, step) / PhysicManager.Constant.AstronomicalDistance) * 10f;
            currentPos = newPos;
            
            positions.Add(PhysicManager.VectorToEngine(newPos));
        }
        lineRenderer.SetPositions(positions.ToArray());
    }
}
