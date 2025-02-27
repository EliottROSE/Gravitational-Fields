using System;
using System.Collections.Generic;
using System.Linq;
using Global;
using UnityEngine;
using UnityEngine.Events;

namespace Global
{
    [Serializable]
    public enum UIState
    {
        HIDDEN,
        CREATION,
        INFORMATION,
        TIME_CONTROL,
        MAIN
    }
}

public class UIManager : MonoBehaviour
{
    #region Event List

    [HideInInspector] public UnityEvent<UIState> onUIStateUpdate;

    #endregion


    #region Unity Callbacks

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        cam = Camera.main;
    }

    #endregion

    #region Parameters

    public GameObject background;

    private static UIManager _instance;
    [HideInInspector] public Camera cam;
    private UIState m_state;

    /*
     * All UI GameObjects
     */
    public List<UIPanel> panelList;
    public List<UIPanel> enabledPanels;

    #endregion


    #region Getters/Setters

    public static UIManager Instance
    {
        get
        {
            if (!_instance)
                Debug.LogError("There is no existing instance of UIManager!");

            return _instance;
        }
    }

    public UIState State
    {
        get => m_state;

        set
        {
            if (m_state == value) return;

            m_state = value;
            onUIStateUpdate.Invoke(m_state);
        }
    }

    #endregion


    #region Custom Methods

    public void SwitchPanel(UIState newState)
    {
        foreach (UIPanel panel in panelList)
            if (panel.State == newState)
                panel.Enable();
            else
                panel.Disable();
    }

    public void EnablePanel(UIState panelState)
    {
        foreach (UIPanel panel in panelList)
        {
            if (panel.State != panelState) continue;
            if (enabledPanels.Contains(panel)) continue;

            panel.Enable();
            enabledPanels.Add(panel);
        }
    }

    public void DisablePanel(UIState panelState)
    {
        foreach (UIPanel panel in enabledPanels.ToList())
        {
            if (panel.State != panelState) continue;

            panel.Disable();
            enabledPanels.Remove(panel);
        }
    }

    #endregion
}