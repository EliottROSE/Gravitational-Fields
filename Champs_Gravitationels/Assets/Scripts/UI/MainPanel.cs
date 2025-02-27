using System.Collections;
using System.Collections.Generic;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : UIPanel
{
    [SerializeField] private Button CreatePlanetButton;
    [SerializeField] private Button EnableSunButton;
    [SerializeField] private TextMeshProUGUI EnableSunText;

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

    private void CreatePlanet()
    {

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
