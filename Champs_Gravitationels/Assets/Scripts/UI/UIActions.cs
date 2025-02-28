using Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIActions : UIPanel
{
    [SerializeField] Button pauseButton;
    [SerializeField] Button changeSceneButton;

    private void Start()
    {
        State = UIState.ACTIONS;
        UIManager.Instance.EnablePanel(State);
    }
    
    public override void Enable()
    {
        pauseButton.onClick.AddListener(Pause);
        changeSceneButton.onClick.AddListener(ChangeScene);
    }

    public override void Disable()
    {
        pauseButton.onClick.RemoveListener(Pause);
        changeSceneButton.onClick.RemoveListener(ChangeScene);
    }

    public void ChangeScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Pause()
    {
        if (PhysicManager.IsPaused)
            PhysicManager.IsPaused = false;
        else
            PhysicManager.IsPaused = true;
    }
}
