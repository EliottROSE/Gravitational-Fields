using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class FieldLines : MonoBehaviour
{
    public int pointsCount = 150;
    public int linesCount = 20;
    public float step = 0.01f;

    private List<LineRenderer> lineRenderers;
    private PhysicManager physicManager;
    private List<Vec3> spherePoints;
    private List<Vector3> startPositions;

    private void Start()
    {
        physicManager = FindObjectOfType<PhysicManager>();
        if (physicManager == null)
            Debug.LogError("Cannot find physic manager on celestial object" + name);

        lineRenderers = new List<LineRenderer>();
        startPositions = new List<Vector3>();
        spherePoints = new List<Vec3>();

        for (int i = 0; i < linesCount; ++i)
        {
            GameObject obj = new();
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();

            if (lineRenderer == null) Debug.LogError("Cannot find line renderer on celestial object" + name);

            lineRenderer.positionCount = pointsCount;

            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.green;

            lineRenderers.Add(lineRenderer);

            startPositions.Add(
                PhysicManager.VectorToSystem(transform.position + Random.onUnitSphere * transform.localScale.x));
            spherePoints.Add(Random.onUnitSphere);
        }
    }

    private void LateUpdate()
    {
        DrawFieldLines();
    }

    private void DrawFieldLines()
    {
        for (int i = 0; i < linesCount; ++i)
        {
            List<Vec3> pointPositions = new(pointsCount);
            
            startPositions[i] = PhysicManager.VectorToSystem(transform.position + spherePoints[i] * transform.localScale.x);
            Vector3 currentPos = startPositions[i];

            for (int j = 0; j < lineRenderers[i].positionCount; ++j)
            {
                pointPositions.Add(PhysicManager.VectorToEngine(currentPos));
                currentPos = physicManager.LineFieldNextPos(currentPos * 0.1f, step) / PhysicManager.Constant.AstronomicalDistance * 10f;
                pointPositions.Add(PhysicManager.VectorToEngine(currentPos));
            }
            lineRenderers[i].SetPositions(pointPositions.ToArray());
        }
    }
}