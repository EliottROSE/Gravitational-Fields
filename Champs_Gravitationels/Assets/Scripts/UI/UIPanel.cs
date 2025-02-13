using UnityEngine;

public class UIPanel : MonoBehaviour
{
    #region Parameters
    public UIState State;
    public GameObject firstButton;
    #endregion

    #region Custom Methods
    public virtual void Enable()
    {  
    }

    public virtual void Disable()
    {
    }
    #endregion
}