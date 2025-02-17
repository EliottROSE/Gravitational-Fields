using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.Events;

namespace Global
{
    [System.Serializable]
    public enum UIState
    {
        SHOWN = -1,
        CREATION,
        INFORMATION
    }
}

public class UIManager : MonoBehaviour
{
    #region Parameters

    public GameObject background;

    private static UIManager instance;
    [HideInInspector] public Camera cam;
    private UIState state;

    /*
     * All UI GameObjects
     */
    public List<UIPanel> panelList;

    #endregion


    #region Getters/Setters

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("There is no existing instance of UIManager!");

            return instance;
        }
    }

    public UIState State
    {
        get => state;

        set
        {
            if (state == value) return;

            state = value;
            onUIStateUpdate.Invoke(state);
        }
    }

    #endregion


    #region Event List

    [HideInInspector] public UnityEvent<UIState> onUIStateUpdate;

    #endregion


    #region Unity Callbacks

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        cam = Camera.main;
    }

    #endregion


    #region Custom Methods

    public void SwitchPanel(UIState newState)
    {
        foreach (var panel in panelList)
        {
            if (panel.State == newState)
                panel.Enable();
            else
                panel.Disable();
        }
    }

    #endregion
}