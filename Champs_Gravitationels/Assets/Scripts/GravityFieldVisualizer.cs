using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GravityFieldVisualizer : MonoBehaviour
{
    public PhysicManager gravityField;
    public CameraController cameraC;
    //public GravityField gField;
    public int density = 10; //Number of point per axis
    public float spacing = 5f; //Spacing between points
    public LayerMask objectMask; //To avoid celestials objects
    private List<LineRenderer> arrows = new List<LineRenderer>();

    public bool is2DMode = false;

    private void Start()
    {

    }

    void GenerateGrid()
    {
        if (is2DMode == true)
        {
            for (float x = cameraC.selectedObject.transform.position.x - density / 2; x <= cameraC.selectedObject.transform.position.x + density / 2; x++)
            {
                for (float z = cameraC.selectedObject.transform.position.z - density / 2; z <= cameraC.selectedObject.transform.position.z + density / 2; z++)
                {
                    Vector3 position = new Vector3(x, 0, z) * spacing;

                    //Check if a celestial body is near
                    if (Physics.CheckSphere(position, spacing / 2, objectMask))
                    {
                        continue;
                    }

                    //Create an arrow
                    LineRenderer line = CreateArrow(position);
                    arrows.Add(line);
                }
            }
        }
        else
        {
            for (float x = cameraC.selectedObject.transform.position.x - density / 2; x <= cameraC.selectedObject.transform.position.x + density / 2; x++)
            {
                for (float y = cameraC.selectedObject.transform.position.y - density / 2; y <= cameraC.selectedObject.transform.position.y + density / 2; y++)
                {
                    for (float z = cameraC.selectedObject.transform.position.z - density / 2; z <= cameraC.selectedObject.transform.position.z + density / 2; z++)
                    {
                        Vector3 position = new Vector3(x , y , z) * spacing;

                        //Check if a celestial body is near
                        if (Physics.CheckSphere(position, spacing / 2, objectMask))
                        {
                            continue;
                        }

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
        line.startWidth = 0.1f;
        line.endWidth = 0.02f;
        line.positionCount = 2;

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
        if(Input.GetKeyDown(KeyCode.T))
        {
            if(is2DMode == true)
            {
                is2DMode = false;
                ClearGrid();
                GenerateGrid();
            }
            else
            {
                is2DMode = true;
                ClearGrid();
                GenerateGrid();
            }
        }
        foreach (LineRenderer arrow in arrows)
        {
            Vector3 position = arrow.transform.position;
            Vector3 gravity = gravityField.TotalGravitionalField(position);

            if(gravity.magnitude > 0.0001f)
            {
                Vector3 normalizedGravity = gravity.normalized;
                float length = Mathf.Min(gravity.magnitude, Mathf.Sqrt(3));

                if(is2DMode)
                {
                    normalizedGravity.y = 0;
                }

                arrow.SetPosition(0, position);
                arrow.SetPosition(1, position + normalizedGravity);
            }
            else
            {
                arrow.SetPosition(0, position);
                arrow.SetPosition(1, position);
            }
        }
    }
}
