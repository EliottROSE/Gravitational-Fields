using Global;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ObjectInfo : UIPanel
    {
        #region Parameters
        
        public TMP_Text objectNameText;
        public TMP_Text xPosText;
        public TMP_Text yPosText;
        public TMP_Text zPosText;
        public TMP_Text xSpeedText;
        public TMP_Text ySpeedText;
        public TMP_Text zSpeedText;
        public TMP_Text sizeText;
        public TMP_Text massText;
        
        private CameraController camController;
        
        #endregion
        
        
        #region Unity Callbacks

        private void Awake()
        {
            State = UIState.INFORMATION; // Update UIState for the UIManager to detect changes
            
            camController = Camera.main?.GetComponent<CameraController>();
        }

        public override void Enable()
        {
            gameObject.SetActive(true);
            UIManager.Instance.background.SetActive(true);

            EventSystem.current.SetSelectedGameObject(FirstButton);
        }

        public override void Disable()
        {
            gameObject.SetActive(false);
            UIManager.Instance.background.SetActive(false);
        }

        #endregion


        #region Custom Methods

        private void UpdateInfo()
        {
        }

        #endregion
    }
}