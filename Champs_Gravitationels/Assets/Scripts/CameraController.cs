using Global;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject grid;
    public CelestialObject selectedObject;
    [SerializeField] private float distance = 10.0f; //Initial distance
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 50.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private float zoomSpeed = 5.0f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lookSpeed = 2f;

    private Transform m_target; //Target of the camera
    private Camera m_cam;
    private float m_azimuth = 0.0f; //Longitude
    private float m_elevation = 20.0f; //Colatitude
    private float m_yaw = 0.0f;
    private float m_pitch = 0.0f;
    private bool m_isOrbitMode = false;
    [SerializeField] private bool m_useStaticCamera = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_cam = Camera.main;
        if(grid != null)
        {
            grid.SetActive(false);//Grille inactive au start
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (m_isOrbitMode && m_target)
        {
            OrbitMode();
        }
        else
        {
            FreeMode();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.CompareTag("TrackingObject"))
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

    private void OrbitMode()
    {
        //Rotation with directional arrows
        m_azimuth += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        m_elevation += Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
        m_elevation = Mathf.Clamp(m_elevation, -80f, 80f);

        //Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(m_elevation, m_azimuth, 0);
        Vector3 position = m_target.position - rotation * Vector3.forward * distance;

        //Apply position and orientation
        if (!m_useStaticCamera)
            transform.position = position;
        
        transform.LookAt(m_target);
        transform.position = position;
        transform.LookAt(target);

        UpdateGravityFieldPos();
    }

    private void FreeMode()
    {
        if (Input.GetMouseButton(1))
        {
            m_yaw += Input.GetAxis("Mouse X") * lookSpeed;
            m_pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
            m_pitch = Mathf.Clamp(m_pitch, -90f, 90f);
        }

        transform.rotation = Quaternion.Euler(m_pitch, m_yaw, 0);

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (moveSpeed * Time.deltaTime);
        transform.position += transform.rotation * move;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        }
    }

    private void SelectObject(Transform newTarget)
    {
        m_target = newTarget;
        m_isOrbitMode = true;

        if (m_target.GetComponent<CelestialObject>())
        {
            selectedObject = m_target.GetComponent<CelestialObject>();
            UIManager.Instance.EnablePanel(UIState.INFORMATION);
        }
        
        CustomEvents.ObjectClicked();
        if(grid != null)
        {
            grid.SetActive(true);
        }
    }

    private void DeselectObject()
    {
        m_isOrbitMode = false;
        m_target = null;
        
        UIManager.Instance.DisablePanel(UIState.INFORMATION);
        
        selectedObject = null;
        if (grid != null)
        {
            grid.SetActive(false);
        }
    }

    void UpdateGravityFieldPos()
    {
        if(grid != null && selectedObject != null)
        {
            grid.transform.position = selectedObject.transform.position;
            grid.transform.position += new Vector3(0, 0, 7f);
        }
    }
}
