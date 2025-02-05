using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
        Cursor.lockState = CursorLockMode.Locked;
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

        if(target == null)
        {
            return;
        }

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
}
