using System.Collections.Generic;
using UnityEngine;

public class GravityFieldVisualizer : MonoBehaviour
{
    public PhysicManager gravityField;
    public CameraController cameraC;
    public int density = 10; //Number of point per axis
    public float gridSize = 5f; //Size of the grid

    public bool is2DMode;
    public bool isFieldVisible;
    private readonly List<LineRenderer> arrows = new();

    private void Start()
    {
        CustomEvents.OnGridCreated += GenerateGrid;
        CustomEvents.OnGridDestroyed += ClearGrid;
    }

    private void LateUpdate()
    {
        UpdateGrid();
    }

    private void OnDisable()
    {
        CustomEvents.OnGridCreated -= GenerateGrid;
        CustomEvents.OnGridDestroyed -= ClearGrid;
    }

    private void UpdateGrid()
    {
        if (!isFieldVisible)
            return;

        foreach (LineRenderer arrow in arrows)
        {
            Vector3 position = arrow.transform.position;
            Vector3 gravity = gravityField.TotalGravitionalField(position);

            if (gravity.magnitude > 0.0001f)
            {
                Vector3 normalizedGravity = gravity.normalized;

                if (is2DMode)
                    normalizedGravity.y = 0;

                arrow.SetPosition(0, position);
                arrow.SetPosition(1, position + normalizedGravity * 0.1f);
            }
            else
            {
                arrow.SetPosition(0, position);
                arrow.SetPosition(1, position);
            }

            //float gravityStrength = Mathf.Clamp01(gravity.magnitude);
            Color arrowColor = Color.Lerp(Color.yellow, Color.red, gravity.magnitude);
            Gradient gradient = new();
            gradient.SetKeys(
                new[] { new GradientColorKey(arrowColor, 0.0f), new GradientColorKey(arrowColor, 1.0f) },
                new[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            arrow.colorGradient = gradient;
        }
    }

    private void GenerateGrid()
    {
        if (is2DMode)
        {
            float spacing = gridSize / (density - 1);
            Vector3 center = cameraC.selectedObject.transform.position;
            for (float i = 0; i < density; i++)
            for (float k = 0; k < density; k++)
            {
                float x = center.x - gridSize / 2 + i * spacing;
                float z = center.z - gridSize / 2 + k * spacing;
                Vector3 position = new(x, 0, z);

                //Create an arrow
                LineRenderer line = CreateArrow(position);
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                line.allowOcclusionWhenDynamic = false;
                arrows.Add(line);
            }
        }
        else
        {
            float spacing = gridSize / (density - 1);
            Vector3 center = cameraC.selectedObject.transform.position;
            for (float i = 0; i < density; i++)
            for (float j = 0; j < density; j++)
            for (float k = 0; k < density; k++)
            {
                float x = center.x - gridSize / 2 + i * spacing;
                float y = center.y - gridSize / 2 + j * spacing;
                float z = center.z - gridSize / 2 + k * spacing;
                Vector3 position = new(x, y, z);

                //Create an arrow
                LineRenderer line = CreateArrow(position);
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                line.allowOcclusionWhenDynamic = false;
                arrows.Add(line);
            }
        }
    }

    private LineRenderer CreateArrow(Vector3 position)
    {
        GameObject arrow = new("Arrow");
        LineRenderer line = arrow.AddComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.002f;
        line.positionCount = 2;

        line.material = new Material(Shader.Find("Sprites/Default"));

        arrow.transform.position = position;
        arrow.transform.parent = transform;

        return line;
    }

    private void ClearGrid()
    {
        foreach (LineRenderer arrow in arrows) Destroy(arrow.gameObject);
        arrows.Clear();
    }
}