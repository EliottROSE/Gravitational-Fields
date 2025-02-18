using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;
using static UnityEngine.Rendering.DebugUI;

public class FieldLines : MonoBehaviour
{
    PhysicManager physicManager;

    List<LineRenderer> lineRenderers;
    List<Vector3> startPositions;
    List<Vec3> spherePoints;

    public int pointsCount = 150;
    public int linesCount = 20;
    public float step = 0.01f;

    void Start()
    {
        physicManager = FindAnyObjectByType<PhysicManager>();
        if (physicManager == null)
            Debug.LogError("Cannot find physic manager on celestial object" + this.name);

        lineRenderers = new List<LineRenderer>();
        startPositions = new List<Vector3>();
        spherePoints = new List<Vec3>();

        for (int i = 0; i < linesCount; ++i)
        {
            GameObject obj = new GameObject();
            LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();

            if (lineRenderer == null)
            {
                Debug.LogError("Cannot find line renderer on celestial object" + this.name);
            }

            lineRenderer.positionCount = pointsCount;

            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;

            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.green;

            lineRenderers.Add(lineRenderer);

            startPositions.Add(PhysicManager.VectorToSystem(transform.position + UnityEngine.Random.onUnitSphere * transform.localScale.x));
            spherePoints.Add(UnityEngine.Random.onUnitSphere);
        }
    }

    void Update()
    {
        DrawFieldLines();
    }

    public void DrawFieldLines()
    {
        for (int i = 0; i < linesCount; ++i)
        {
            startPositions[i] = PhysicManager.VectorToSystem(transform.position + spherePoints[i] * transform.localScale.x);
            Vector3 currentPos = startPositions[i];
            List<Vec3> positions = new List<Vec3>();
            for (int j = 0; j < lineRenderers[i].positionCount; ++j)
            {
                positions.Add(PhysicManager.VectorToEngine(currentPos));

                Vector3 newPos = (physicManager.LineFieldNextPos(currentPos / 10f, step) / PhysicManager.Constant.AstronomicalDistance) * 10f;
                currentPos = newPos;

                positions.Add(PhysicManager.VectorToEngine(newPos));
            }
            lineRenderers[i].SetPositions(positions.ToArray());
        }
    }
}
