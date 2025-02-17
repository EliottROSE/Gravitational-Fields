using System.ComponentModel;
using Global;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    #region Parameters
    
    [ReadOnly(true)] public UIState State;
    public GameObject FirstButton;
    
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