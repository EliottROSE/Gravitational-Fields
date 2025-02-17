using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GravityFieldVisualizer : MonoBehaviour
{
    public PhysicManager gravityField;
    //public GravityField gField;
    public int density = 10; //Number of point per axis
    public float spacing = 5f; //Spacing between points
    public float arrowScale = 1f; //Maximum arrow scale
    public LayerMask objectMask; //To avoid celestials objects
    private List<LineRenderer> arrows = new List<LineRenderer>();

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for(int x = 0; x < density / 2; x++)
        {
            for (int y = 0; y < density / 2; y++)
            {
                for (int z = 0; z < density / 2; z++)
                {
                    Vector3 position = transform.position + new Vector3(x - density / 2, y - density / 2, z - density / 2) * spacing;

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

    private void Update()
    {
        foreach (LineRenderer arrow in arrows)
        {
            Vector3 position = arrow.transform.position;
            Vector3 gravity = PhysicManager.VectorToEngine(gravityField.TotalGravitionalField(PhysicManager.VectorToSystem(position)));

            if(gravity.magnitude > 0.0001f)
            {
                Vector3 normalizedGravity = gravity.normalized;
                float length = Mathf.Min(gravity.magnitude * arrowScale, spacing * 0.5f);

                arrow.SetPosition(0, position);
                arrow.SetPosition(1, position + normalizedGravity * length);
            }
            else
            {
                arrow.SetPosition(0, position);
                arrow.SetPosition(1, position);
            }
        }
    }
}
