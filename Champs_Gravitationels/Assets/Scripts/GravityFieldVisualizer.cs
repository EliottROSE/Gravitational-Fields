using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class GravityFieldVisualizer : MonoBehaviour
{
    public PhysicManager gravityField;
    public CameraController cameraC;
    public int density = 10; //Number of point per axis
    public float gridSize = 5f; //Size of the grid
    public LayerMask objectMask; //To avoid celestials objects
    private List<LineRenderer> arrows = new List<LineRenderer>();

    public bool is2DMode = false;
    public bool isFieldVisible = false;

    public UnityEvent SetFieldModeEvent;

    private void Start()
    {
        //if (SetFieldModeEvent == null)
        //{
        //    SetFieldModeEvent = new UnityEvent();
        //    SetFieldModeEvent.AddListener(GenerateGrid);
        //}
        //GenerateGrid();
    }

    void GenerateGrid()
    {
        if (is2DMode == true)
        {
            float spacing = gridSize / (density - 1);
            Vector3 center = cameraC.selectedObject.transform.position;
            for (float i = 0; i < density; i++)
            {
                for (float k = 0; k < density; k++)
                {
                    float x = center.x - gridSize / 2 + i * spacing;
                    float z = center.z - gridSize / 2 + k * spacing;
                    Vector3 position = new Vector3(x, 0, z);

                    //Create an arrow
                    LineRenderer line = CreateArrow(position);
                    arrows.Add(line);
                }
            }
        }
        else
        {
            float spacing = gridSize / (density - 1);
            Vector3 center = cameraC.selectedObject.transform.position;
            for (float i = 0; i < density; i++)
            {
                for (float j = 0; j < density; j++)
                {
                    for (float k = 0; k < density; k++)
                    {
                        float x = center.x - gridSize / 2 + i * spacing;
                        float y = center.y - gridSize / 2 + j * spacing;
                        float z = center.z - gridSize / 2 + k * spacing;
                        Vector3 position = new Vector3(x , y , z);

                        //Create an arrow
                        LineRenderer line = CreateArrow(position);
                        arrows.Add(line);
                    }
                }
            }
        }
    }

    LineRenderer CreateArrow(Vector3 position)
    {
        GameObject arrow = new GameObject("Arrow");
        LineRenderer line = arrow.AddComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.002f;
        line.positionCount = 2;

        line.material = new Material(Shader.Find("Sprites/Default"));
        
        arrow.transform.position = position;
        arrow.transform.parent = transform;

        return line;
    }

    void ClearGrid()
    {
        foreach(LineRenderer arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }
        arrows.Clear();
    }

    void LateUpdate()
    {
        if (isFieldVisible)
        {
            if (is2DMode == true)
            {
                ClearGrid();
                GenerateGrid();
            }
            else
            {
                ClearGrid();
                GenerateGrid();
            }
        }
        else
        {
            ClearGrid();
        }

        foreach (LineRenderer arrow in arrows)
        {
            Vector3 position = arrow.transform.position;
            Vector3 gravity = gravityField.TotalGravitionalField(position);

            if (gravity.magnitude > 0.0001f)
            {
                Vector3 normalizedGravity = gravity.normalized;

                if(is2DMode)
                {
                    normalizedGravity.y = 0;
                }

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
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(arrowColor, 0.0f), new GradientColorKey(arrowColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            arrow.colorGradient = gradient;
        }
    }
}
