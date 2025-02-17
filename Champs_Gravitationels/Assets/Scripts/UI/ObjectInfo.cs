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
        private CelestialObject selectedObj;
        
        #endregion
        
        
        #region Unity Callbacks

        private void Awake()
        {
            State = UIState.INFORMATION; // Update UIState for the UIManager to detect changes
            
            camController = Camera.main?.GetComponent<CameraController>();
            selectedObj = camController?.selectedObject != null ? camController.selectedObject : null;
        }

        public override void Enable()
        {
            //UIManager.Instance.background.SetActive(true);

            EventSystem.current.SetSelectedGameObject(FirstButton);
            UpdateInfo();
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
            if (selectedObj == null)
                return;
            
            objectNameText.text = selectedObj.name;
            xPosText.text = selectedObj.position.x.ToString();
            yPosText.text = selectedObj.position.y.ToString();
            zPosText.text = selectedObj.position.z.ToString();
            xSpeedText.text = selectedObj.msSpeed.X + "m/s";
            ySpeedText.text = selectedObj.msSpeed.Y + "m/s";
            zSpeedText.text = selectedObj.msSpeed.Z + "m/s";
            sizeText.text = selectedObj.gameObject.transform.localScale.ToString();
            massText.text = selectedObj.kgMass.ToString();
        }

        #endregion
    }
}