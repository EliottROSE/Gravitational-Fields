using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

using Vector3 = System.Numerics.Vector3;
using Vec3 = UnityEngine.Vector3;

public class MainPanel : UIPanel
{
    [SerializeField] private UnityEngine.UI.Button CreatePlanetButton;
    [SerializeField] private UnityEngine.UI.Button EnableSunButton;
    [SerializeField] private TextMeshProUGUI EnableSunText;

    [SerializeField] private TMP_InputField xPosInput;
    [SerializeField] private TMP_InputField yPosInput;
    [SerializeField] private TMP_InputField zPosInput;

    [SerializeField] private TMP_InputField xSpeedInput;
    [SerializeField] private TMP_InputField ySpeedInput;
    [SerializeField] private TMP_InputField zSpeedInput;

    [SerializeField] private TMP_InputField sizeInput;
    [SerializeField] private TMP_InputField massInput;

    private PhysicManager physicManager;

    public void Awake()
    {
        State = UIState.MAIN;
    }

    public override void Enable()
    {
        physicManager = FindObjectOfType<PhysicManager>();
        if (!physicManager)
        {
            Debug.LogError("Can't find physic manager in Main panel");
        }

        CreatePlanetButton.onClick.AddListener(CreatePlanet);
        EnableSunButton.onClick.AddListener(EnableSun);
    }

    public override void Disable()
    {
        CreatePlanetButton.onClick.RemoveListener(CreatePlanet);
        EnableSunButton.onClick.RemoveListener(EnableSun);
    }



    void Update()
    {
        
    }

    public Vector3 CreatePos()
    {
        if (!float.TryParse(xPosInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueX))
        {
            xPosInput.text = ""; 
        }

        if (!float.TryParse(yPosInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueY))
        {
            yPosInput.text = "";
        }

        if (!float.TryParse(zPosInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueZ))
        {
            zPosInput.text = "";
        }

        Vector3 newPos = new Vector3(valueX, valueY, valueZ);
        return newPos;
    }

    public Vector3 CreateSpeed()
    {
        if (!float.TryParse(xSpeedInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueX))
        {
            xSpeedInput.text = "";
        }

        if (!float.TryParse(ySpeedInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueY))
        {
            ySpeedInput.text = "";
        }

        if (!float.TryParse(zSpeedInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueZ))
        {
            zSpeedInput.text = "";
        }
        Vector3 newSpeed = new Vector3(valueX, valueY, valueZ);
        return newSpeed;
    }

    public float CreateMass()
    {
        if (!float.TryParse(massInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
            massInput.text = "";
        }

        return value;
    }

    public Vector3 CreateSize()
    {
        if (!float.TryParse(sizeInput.text, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
        {
            sizeInput.text = "";
        }
        Vector3 size = new Vector3(value, value, value);
        return size;
    }

    public void CreatePlanet()
    {
        Vector3 pos = CreatePos();
        Vector3 speed = CreateSpeed();
        float mass = CreateMass();
        Vector3 size = CreateSize();

        physicManager.AddPlanet(pos, speed, mass, size);
    }

    private void EnableSun()
    {
        if (physicManager.bIgnoreSun == true)
        {
            physicManager.bIgnoreSun = false;
            EnableSunText.text = "On";
        }
        else 
        {
            physicManager.bIgnoreSun = true;
            EnableSunText.text = "Off";
        }
    }
}
