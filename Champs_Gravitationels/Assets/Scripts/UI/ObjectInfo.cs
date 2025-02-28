using System.Collections;
using System.Globalization;
using Global;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = System.Numerics.Vector3;

public class ObjectInfo : UIPanel
{
    #region Parameters

    [Header("Text Boxes")] [SerializeField]
    private TMP_Text objectNameText;

    [SerializeField] private TMP_InputField xPosInput;
    [SerializeField] private TMP_InputField yPosInput;
    [SerializeField] private TMP_InputField zPosInput;
    [SerializeField] private TMP_InputField xSpeedInput;
    [SerializeField] private TMP_InputField ySpeedInput;
    [SerializeField] private TMP_InputField zSpeedInput;
    [SerializeField] private TMP_InputField sizeInput;
    [SerializeField] private TMP_InputField massInput;
    [SerializeField] private Button fieldInput;
    [SerializeField] private Button LineInput;
    [SerializeField] private Button LineDimensionInput;
    [SerializeField] private Slider sliderFieldSize;
    [SerializeField] private Slider sliderFieldDensity;

    private TextMeshProUGUI m_xPosPlaceholder;
    private TextMeshProUGUI m_yPosPlaceholder;
    private TextMeshProUGUI m_zPosPlaceholder;
    private TextMeshProUGUI m_xSpeedPlaceholder;
    private TextMeshProUGUI m_ySpeedPlaceholder;
    private TextMeshProUGUI m_zSpeedPlaceholder;
    private TextMeshProUGUI m_sizePlaceholder;
    private TextMeshProUGUI m_massPlaceholder;

    [Header("Parameters")] [Tooltip("Data refresh rate in seconds.")] [SerializeField]
    private float dataRefreshRate = 0.25f;

    private CameraController m_camController;
    private CelestialObject m_selectedObj;
    private GravityFieldVisualizer m_visualizer;

    #endregion


    #region Unity Callbacks

    private void Awake()
    {
        // Update UIState for the UIManager to detect changes
        State = UIState.INFORMATION;

        // Define private variables required
        m_camController = Camera.main?.GetComponent<CameraController>();
        UpdateSelectedObject();
    }

    private void Start()
    {
        m_visualizer = FindAnyObjectByType<GravityFieldVisualizer>();
    }

    public override void Enable()
    {
        CustomEvents.OnCelestialObjectClicked += UpdateSelectedObject;

        m_xPosPlaceholder = xPosInput.placeholder as TextMeshProUGUI;
        m_yPosPlaceholder = yPosInput.placeholder as TextMeshProUGUI;
        m_zPosPlaceholder = zPosInput.placeholder as TextMeshProUGUI;
        m_xSpeedPlaceholder = xSpeedInput.placeholder as TextMeshProUGUI;
        m_ySpeedPlaceholder = ySpeedInput.placeholder as TextMeshProUGUI;
        m_zSpeedPlaceholder = zSpeedInput.placeholder as TextMeshProUGUI;
        m_sizePlaceholder = sizeInput.placeholder as TextMeshProUGUI;
        m_massPlaceholder = massInput.placeholder as TextMeshProUGUI;
        fieldInput.onClick.AddListener(SetField);
        LineInput.onClick.AddListener(SetLine);
        LineDimensionInput.onClick.AddListener(SetFieldDimension);
        sliderFieldSize.onValueChanged.AddListener(ChangeFieldSize);
        sliderFieldDensity.onValueChanged.AddListener(ChangeFieldDensity);
        /*
         * Must call gameobject twice to enable all children before enabling the actual parent
         * Otherwise, causes a NullReferenceException
         */
        gameObject.gameObject.SetActive(true);

        /*
         * Begin object info refreshing every 2s
         * Must be done after the GameObject has been enabled
         */
        StartCoroutine(DataUpdateLoop());
        
    }

    public override void Disable()
    {
        if (m_selectedObj.TryGetComponent(out FieldLines lines) && lines.Active)
        {
            lines.Active = false;
            lines.RemoveLines();
        }

        CustomEvents.OnCelestialObjectClicked -= UpdateData;

        StopCoroutine(DataUpdateLoop());

        fieldInput.onClick.RemoveListener(SetField);
        LineInput.onClick.RemoveListener(SetLine);
        LineDimensionInput.onClick.RemoveListener(SetFieldDimension);
        sliderFieldSize.onValueChanged.RemoveListener(ChangeFieldSize);
        sliderFieldDensity.onValueChanged.RemoveListener(ChangeFieldDensity);
        gameObject.SetActive(false);
    }

    #endregion


    #region Custom Methods

    private IEnumerator DataUpdateLoop()
    {
        if (!m_selectedObj)
            yield break;

        while (true)
        {
            UpdateData();

            yield return new WaitForSeconds(dataRefreshRate);
        }
    }

    private void UpdateData()
    {
        if (!m_selectedObj) return;
        objectNameText.SetText(m_selectedObj.objectName);
        m_xPosPlaceholder.SetText(m_selectedObj.transform.position.x + " ua");
        m_yPosPlaceholder.SetText(m_selectedObj.transform.position.y + " ua");
        m_zPosPlaceholder.SetText(m_selectedObj.transform.position.z + " ua");
        m_xSpeedPlaceholder.SetText(m_selectedObj.kmsSpeed.x + " km/s");
        m_ySpeedPlaceholder.SetText(m_selectedObj.kmsSpeed.y + " km/s");
        m_zSpeedPlaceholder.SetText(m_selectedObj.kmsSpeed.z + " km/s");
        m_sizePlaceholder.SetText(m_selectedObj.gameObject.transform.localScale.ToString());
        m_massPlaceholder.SetText(m_selectedObj.mass.ToString(CultureInfo.InvariantCulture));
    }

    private void UpdateSelectedObject()
    {
        m_selectedObj = m_camController?.selectedObject ? m_camController.selectedObject : null;
        UpdateData();
    }

    public void UpdateXPos(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
            xPosInput.text = "";
            return;
        }

        value *= PhysicManager.Constant.AstronomicalDistance;

        Vector3 newPos = new(value, m_selectedObj.transform.position.y, m_selectedObj.transform.position.z);
        UpdatePos(newPos, xPosInput);
    }

    public void UpdateYPos(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
            yPosInput.text = "";
            return;
        }

        value *= PhysicManager.Constant.AstronomicalDistance;

        Vector3 newPos = new(m_selectedObj.transform.position.x, value, m_selectedObj.transform.position.z);
        UpdatePos(newPos, yPosInput);
    }

    public void UpdateZPos(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
            zPosInput.text = "";
            return;
        }

        value *= PhysicManager.Constant.AstronomicalDistance;

        Vector3 newPos = new(m_selectedObj.transform.position.x, m_selectedObj.transform.position.y, value);
        UpdatePos(newPos, zPosInput);
    }

    private void UpdatePos(Vector3 newPos, TMP_InputField inputField)
    {
        m_selectedObj.AstronomicalPos = newPos;
        inputField.text = "";
    }

    public void UpdateXSpeed(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
            xSpeedInput.text = "";
            return;
        }

        Vector3 newSpeed = new(value, m_selectedObj.kmsSpeed.y, m_selectedObj.kmsSpeed.z);
        UpdateSpeed(newSpeed, xSpeedInput);
    }

    private void UpdateSpeed(Vector3 newSpeed, TMP_InputField inputField)
    {
        m_selectedObj.kmsSpeed = PhysicManager.VectorToEngine(newSpeed);
        inputField.text = "";
    }

    private void SetField()
    {
        if (m_visualizer.isFieldVisible)
        {
            m_visualizer.isFieldVisible = false;
            CustomEvents.GridDeletion();
        }
        else
        {
            m_visualizer.isFieldVisible = true;
            CustomEvents.GridCreation();
        }
    }

    private void SetFieldDimension()
    {
        m_visualizer.is2DMode = !m_visualizer.is2DMode;
        
        CustomEvents.GridDeletion();
        CustomEvents.GridCreation();
    }

    private void SetLine()
    {
        m_selectedObj.TryGetComponent(out FieldLines selectedFieldLines);

        if (!selectedFieldLines) return;

        if (selectedFieldLines.Active)
        {
            selectedFieldLines.Active = false;
            selectedFieldLines.RemoveLines();
        }
        else
        {
            selectedFieldLines.Active = true;
            selectedFieldLines.SetupFieldLines();
        }
    }

    private void ChangeFieldSize(float newValue)
    {
        GravityFieldVisualizer visualizer = FindAnyObjectByType<GravityFieldVisualizer>();
        visualizer.gridSize = newValue;
    }

    private void ChangeFieldDensity(float newValue)
    {
        GravityFieldVisualizer visualizer = FindAnyObjectByType<GravityFieldVisualizer>();
        visualizer.density = (int)newValue;
    }

    #endregion
}