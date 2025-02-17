using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CelestialObject selectedObject;
    Transform target; //Target of the camera
    float distance = 10.0f; //Initial distance
    float minDistance = 2.0f;
    float maxDistance = 50.0f;
    float rotationSpeed = 100.0f;
    float zoomSpeed = 5.0f;
    float moveSpeed = 10f;
    float lookSpeed = 2f;

    private bool isOrbitMode = false;
    private float azimuth = 0.0f; //Longitude
    private float elevation = 20.0f; //Colatitude
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(isOrbitMode && target != null)
        {
            OrbitMode();
        }
        else
        {
            FreeMode();
        }

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.CompareTag("TrackingObject"))
                {
                    SelectObject(hit.transform);
                }
            }
            else
            {
                DeselectObject();
            }
        }
    }

    void OrbitMode()
    {
        //Rotation with directional arrows
        azimuth += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        elevation += Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
        elevation = Mathf.Clamp(elevation, -80f, 80f);

        //Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(elevation, azimuth, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        //Apply position and orientation
        transform.position = position;
        transform.LookAt(target);
    }

    void FreeMode()
    {
        if(Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * lookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
            pitch = Mathf.Clamp(pitch, -90f, 90f);
        }

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
        transform.position += transform.rotation * move;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
    }

    void SelectObject(Transform newTarget)
    {
        target = newTarget;
        isOrbitMode = true;
        if(target.GetComponent<CelestialObject>() != null)
        {
            selectedObject = target.GetComponent<CelestialObject>();
        }
    }

    void DeselectObject()
    {
        isOrbitMode = false;
        target = null;
        selectedObject = null;
    }
}
