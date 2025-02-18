using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GravityFieldVisualizer : MonoBehaviour
{
    public PhysicManager gravityField;
    //public GravityField gField;
    public int density = 10; //Number of point per axis
    public float spacing = 5f; //Spacing between points
    public LayerMask objectMask; //To avoid celestials objects
    private List<LineRenderer> arrows = new List<LineRenderer>();

    public bool is2DMode = false;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        if (is2DMode == true)
        {
            for (int x = -density / 2; x <= density / 2; x++)
            {
                for (int z = -density / 2; z <= density / 2; z++)
                {
                    Vector3 position = new Vector3(x, 0, z) * spacing;

                    //Check if a celestial body is near
                    if (Physics.CheckSphere(position, spacing / 2, objectMask))
                    {
                        continue;
                    }

                    //Create an arrow
                    GameObject arrow = new GameObject("Arrow");
                    LineRenderer line = arrow.AddComponent<LineRenderer>();
                    line.startWidth = 0.1f;
                    line.endWidth = 0.02f;
                    line.positionCount = 2;

                    arrow.transform.position = position;
                    arrow.transform.parent = transform;
                    arrows.Add(line);
                }
            }
        }
        else
        {
            for (int x = -density / 2; x <= density / 2; x++)
            {
                for (int y = -density / 2; y <= density / 2; y++)
                {
                    for (int z = -density / 2; z <= density / 2; z++)
                    {
                        Vector3 position = new Vector3(x , y , z ) * spacing;

                        //Check if a celestial body is near
                        if (Physics.CheckSphere(position, spacing / 2, objectMask))
                        {
                            continue;
                        }

                        //Create an arrow
                        GameObject arrow = new GameObject("Arrow");
                        LineRenderer line = arrow.AddComponent<LineRenderer>();
                        line.startWidth = 0.1f;
                        line.endWidth = 0.02f;
                        line.positionCount = 2;

                        arrow.transform.position = position;
                        arrow.transform.parent = transform;
                        arrows.Add(line);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
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
