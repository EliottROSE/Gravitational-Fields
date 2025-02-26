using System.Collections;
using System.Globalization;
using Global;
using TMPro;
using UnityEngine;
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
        CustomEvents.OnCelestialObjectClicked -= UpdateData;

        StopCoroutine(DataUpdateLoop());

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
        objectNameText.SetText(m_selectedObj.objectName);
        m_xPosPlaceholder.SetText(m_selectedObj.transform.position.x / 10f + " ua");
        m_yPosPlaceholder.SetText(m_selectedObj.transform.position.y / 10f + " ua");
        m_zPosPlaceholder.SetText(m_selectedObj.transform.position.z / 10f + " ua");
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
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float inputVal))
            return;

        inputVal *= PhysicManager.Constant.AstronomicalDistance;

        var newPos = new Vector3(inputVal, m_selectedObj.transform.position.y, m_selectedObj.transform.position.z);
        UpdatePos(newPos, xPosInput);
    }

    public void UpdateYPos(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float inputVal))
            return;

        inputVal *= PhysicManager.Constant.AstronomicalDistance;

        var newPos = new Vector3(m_selectedObj.transform.position.x, inputVal, m_selectedObj.transform.position.z);
        UpdatePos(newPos, yPosInput);
    }

    public void UpdateZPos(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float inputVal))
            return;

        inputVal *= PhysicManager.Constant.AstronomicalDistance;

        var newPos = new Vector3(m_selectedObj.transform.position.x, m_selectedObj.transform.position.y, inputVal);
        UpdatePos(newPos, zPosInput);
    }

    private void UpdatePos(Vector3 newPos, TMP_InputField inputField)
    {
        m_selectedObj.AstronomicalPos = newPos;
        inputField.text = "";
    }

    public void UpdateXSpeed(string input)
    {
        if (!float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out float inputVal))
            return;

        inputVal *= PhysicManager.Constant.KmPerSecToMeterPerSec / 1000f;

        var newSpeed = new Vector3(inputVal, m_selectedObj.kmsSpeed.y, m_selectedObj.kmsSpeed.z);
        UpdateSpeed(newSpeed, xSpeedInput);
    }

    private void UpdateSpeed(Vector3 newSpeed, TMP_InputField inputField)
    {
        m_selectedObj.msSpeed = newSpeed;
        inputField.text = "";
    }

    #endregion
}