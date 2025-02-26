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
    List<Vec3> startPositions;
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
        startPositions = new List<Vec3>();
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

            startPositions.Add(transform.position + UnityEngine.Random.onUnitSphere * transform.localScale.x);
            spherePoints.Add(UnityEngine.Random.onUnitSphere);
        }
    }

    void Update()
    {
        DrawFieldLines();
    }

    public void DrawFieldLines()
    {
        //Vector3 position = arrow.transform.position;
        //Vector3 gravity = gravityField.TotalGravitionalField(position);

        //if (gravity.magnitude > 0.0001f)
        //{
        //    Vector3 normalizedGravity = gravity.normalized;
        //    float length = Mathf.Min(gravity.magnitude, Mathf.Sqrt(3));

        //    if (is2DMode)
        //    {
        //        normalizedGravity.y = 0;
        //    }

        //    arrow.SetPosition(0, position);
        //    arrow.SetPosition(1, position + normalizedGravity);
        //}
        for (int i = 0; i < linesCount; ++i)
        {
            startPositions[i] = transform.position + spherePoints[i] * transform.localScale.x;
            Vec3 currentPos = startPositions[i];
            List<Vec3> positions = new List<Vec3>();
            positions.Add(currentPos);

            for (int j = 0; j < lineRenderers[i].positionCount - 1; ++j)
            {
                Vec3 totalField = -physicManager.TotalGravitionalField(currentPos);
                if (totalField.magnitude < 0.0001f)
                    continue;
                Vec3 newPos = currentPos + (totalField / totalField.magnitude) * step;
                positions.Add(newPos);
                currentPos = newPos;
            }
            lineRenderers[i].SetPositions(positions.ToArray());
        }
    }
}
