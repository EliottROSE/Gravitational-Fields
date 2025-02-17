using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFieldVisualizer : MonoBehaviour
{
    //public PhysicManager gravityField;
    public GravityField gField;
    public int density = 10; //Number of point per axis
    public float spacing = 5f; //Spacing between points
    public float arrowScale = 1f; //Maximum arrow scale
    public LayerMask objectMask; //To avoid celestials objects
    private List<GameObject> arrows = new List<GameObject>();

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for(int x = -density / 2; x < density / 2; x++)
        {
            for (int y = -density / 2; y < density / 2; y++)
            {
                for (int z = -density / 2; z < density / 2; z++)
                {
                    Vector3 position = transform.position + new Vector3(x, y, z) * spacing;

                    //Check if a celestial body is near
                    if (Physics.CheckSphere(position, spacing / 2, objectMask))
                    {
                        continue;
                    }

                    //Create an arrow
                    GameObject arrow = new GameObject("Arrow");
                    LineRenderer line = arrow.AddComponent<LineRenderer>();
                    line.startWidth = 1f;
                    line.endWidth = 0.15f;
                    line.positionCount = 2;

                    arrow.transform.parent = transform;
                    arrows.Add(arrow);
                }
            }
        }
    }

    private void Update()
    {
        for(int i = 0; i < arrows.Count; i++)
        {
            GameObject arrow = arrows[i];
            Vector3 position = arrow.transform.position;
            //Vector3 gravity = PhysicManager.VectorToEngine(gravityField.TotalGravitionalField(PhysicManager.VectorToSystem(position)));
            Vector3 gravity = gField.ComputeGravity(position);

            float magnitude = gravity.magnitude;
            if(magnitude > arrowScale)
            {
                gravity = gravity.normalized * arrowScale;
            }

            LineRenderer line = arrow.GetComponent<LineRenderer>();
            line.SetPosition(0, position);
            line.SetPosition(1, position + gravity);
        }
    }
}
