using System.Collections.Generic;
using UnityEngine;
using Vec3 = UnityEngine.Vector3;

public class FieldLines : MonoBehaviour
{
    public int pointsCount = 150;
    public int linesCount = 20;
    public float step = 0.01f;

    private List<LineRenderer> lineRenderers;

    private PhysicManager physicManager;
    private List<Vec3> spherePoints;
    private List<Vec3> startPositions;

    public bool Active = false;

    private void Start()
    {
        physicManager = FindObjectOfType<PhysicManager>();
        if (physicManager == null)
            Debug.LogError("Cannot find physic manager on celestial object" + name);
    }

    public void SetupFieldLines()
    {
        if (physicManager.lineRenderers.Count != 0)
        {
            foreach (LineRenderer lineRenderer in physicManager.lineRenderers)
            { 
                lineRenderer.enabled = true;
            }
            lineRenderers = physicManager.lineRenderers;
            spherePoints = physicManager.spherePoints;
            startPositions = physicManager.startPositions;
        }
        else
        {
            lineRenderers = new List<LineRenderer>(linesCount);
            startPositions = new List<Vec3>(linesCount);
            spherePoints = new List<Vec3>(linesCount);

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

                startPositions.Add(transform.position + Random.onUnitSphere * transform.localScale.x);
                spherePoints.Add(Random.onUnitSphere);
            }

            physicManager.lineRenderers = lineRenderers;
            physicManager.spherePoints = spherePoints;
            physicManager.startPositions = startPositions;
        }


    }

    public void RemoveLines()
    {
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }
        }

        physicManager.lineRenderers = lineRenderers;
        physicManager.spherePoints = spherePoints;
        physicManager.startPositions = startPositions;

    }

    private void LateUpdate()
    {
        if (Active)
        {
            DrawFieldLines();
        }
    }

    private void DrawFieldLines()
    {
        for (int i = 0; i < linesCount; ++i)
        {
            startPositions[i] = transform.position + spherePoints[i] * transform.localScale.x;
            Vec3 currentPos = startPositions[i];
            List<Vec3> positions = new(pointsCount) { currentPos };

            for (int j = 0; j < lineRenderers[i].positionCount - 1; ++j)
            {
                Vec3 totalField = -physicManager.TotalGravitionalField(currentPos);
                if (totalField.magnitude < 0.0001f)
                    continue;

                currentPos += totalField / totalField.magnitude * step;
                positions.Add(currentPos);
            }

            lineRenderers[i].SetPositions(positions.ToArray());
        }
    }
}